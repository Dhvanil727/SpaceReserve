using System.Reflection;
using log4net;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Enum;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.AppService.Services;

public class BackgroundTaskService : IBackgroundTaskService
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(BackgroundTaskService));
    private readonly IBackgroundTaskRepository _backgroundTaskRepository;
    private readonly IEmailService _emailService;

    public BackgroundTaskService(IBackgroundTaskRepository backgroundTaskRepository ,IEmailService emailService)
    {
        _emailService = emailService;
        _backgroundTaskRepository = backgroundTaskRepository;
    }

    
    public async Task<bool> ProcessPendingBookings()
    {
        try
        {
            var now = DateTime.Now;
            var nextRun = now.AddMinutes(30);

            _logger.Info($"job is started at {now} and will run next at {nextRun}");

            _logger.Info("Getting list of pending bookings request...");
            var pendingBookings = await _backgroundTaskRepository.GetPendingBookings();

            if (pendingBookings == null || !pendingBookings.Any())
            {
                _logger.Info("No pending request found to auto approve...");
                return false;
            }
            
            var filteredBookings = GetFilteredBookings(pendingBookings.ToList());
            var bookingsToApproved = filteredBookings.bookingsToApproved;
            var bookingsToReject = filteredBookings.bookingsToReject;

            _logger.Info($"Found {bookingsToApproved.Count} bookings to approve and {bookingsToReject.Count} bookings to reject.");
            var approveResult = await _backgroundTaskRepository.AutoApprovePendingBookings(bookingsToApproved);
            _logger.Info("Auto approve pending booking requests completed...");

            if (bookingsToReject.Any())
            {
                await _backgroundTaskRepository.AutoRejectPendingBookings(bookingsToReject);  
                _logger.Info("Auto Reject pending booking requests completed...");
                
            }
            
            //email sending procces start from here 
            //getting seats id to find hybrid owners
            List<short> seatIds = pendingBookings.Select(b => b.SeatId).Distinct().ToList();

            await SendApprovalEmail(bookingsToApproved , seatIds);
            await SendRejectionEmail(bookingsToApproved , bookingsToReject);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while processing pending bookings.", ex);
            return false;
        }
    }


    private async Task<List<(short SeatId , int UserId)>?> GetSeatOwnersBySeatId(List<short> seatIds)
    {
        try
        {
            var seatOwners = await _backgroundTaskRepository.GetSeatOwnersBySeatId(seatIds);
            return seatOwners;
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while getting seat owners by seat ID.", ex);
            return null;
        }
    }

    private (List<Booking> bookingsToApproved , List<Booking?> bookingsToReject) GetFilteredBookings(List<Booking> pendingBookings)
    {
        var sortedBookings = pendingBookings.OrderBy(b => (b.SeatId, b.CreatedDate )).ToList();
            List<Booking> bookingsToApproved = new List<Booking>();
            List<Booking?> bookingsToReject  = new List<Booking?>();

            foreach (var booking in sortedBookings)
            {
                bool seatAlreadyApproved = bookingsToApproved
                    .Any(b => b.SeatId == booking.SeatId && b.BookingDate == booking.BookingDate);

                bool userAlreadyHasBooking = bookingsToApproved
                    .Any(b => b.UserId == booking.UserId && b.BookingDate == booking.BookingDate);

                if (!seatAlreadyApproved && !userAlreadyHasBooking)
                {
                    booking.BookingStatusId =  (byte)BookingStatus.Accepted;
                    booking.ModifiedDate = DateTime.Now;
                    booking.ModifiedBy = 1; 
                    bookingsToApproved.Add(booking);  
                }
                else
                {
                    booking.BookingStatusId =  (byte)BookingStatus.Rejected;
                    booking.ModifiedDate = DateTime.Now;
                    booking.ModifiedBy = 1; 
                    bookingsToReject.Add(booking);
                }
           }
               
        return (bookingsToApproved , bookingsToReject);
        
    }
    private async Task<bool> SendEmail(string subject , string dear , string heading , string automsg, Booking booking , string endMessage , List<int>? adminTo = null )
    {
        
        EmailDto emailDto = new EmailDto
        {
            Subject = $"{subject}",
            Body = $@"
            <html>
            <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ margin: 20px; }}
                .footer {{ margin-top: 30px; font-size: 14px; color: #555; }}
            </style>
            </head>
            <body>
            <div class=""container"">
                <p>Dear {dear},</p>

                <p>{heading}</p>

                <p><strong>Details:</strong></p>
                <ul>
                <li><strong>Date:</strong> Auto {automsg}</li>
                <li><strong>Date:</strong> {booking.BookingDate}</li>
                <li><strong>City:</strong> {booking.Seat!.ColumnModel!.FloorModel!.CityModel!.City}</li>
                <li><strong>Floor:</strong> {booking.Seat!.ColumnModel!.FloorModel.Floor}</li>
                <li><strong>Seat:</strong> {booking.Seat.ColumnModel.Column }{ booking.Seat.SeatNumber}</li>
                </ul>

                <p>{endMessage}</p>

                <div class=""footer"">
                <p>Thank you for using Space Reserve.</p>
                <p>Regards,<br>Space Reserve Team</p>
                </div>
            </div>
            </body>
            </html>",
            ToUserId = adminTo?.Count > 0 ? adminTo : new List<int> { booking.UserId },
        };

        try
        {
           await _emailService.SendEmailAsync(emailDto);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while sending auto proccess email to user.", ex);
            return false;
        }

        return true;
    }
      
    private async Task<bool> SendApprovalEmail(List<Booking> bookingsToApproved , List<short> allSeatIds)
    {
            //getting hybrid seats owners by their seats id
            var hybridSeatOwnersIds =   await GetSeatOwnersBySeatId(allSeatIds); 
 
            try
            {
                foreach (var booking in bookingsToApproved)
                {
                    var hybridOwner = hybridSeatOwnersIds?.FirstOrDefault(s => s.SeatId == booking.SeatId);
                    var admins = await _backgroundTaskRepository.GetAdminsId();
                    if (hybridOwner.HasValue && hybridOwner.Value.UserId != 0 )
                    {
                        var seatOwner = await _backgroundTaskRepository.GetUserById(hybridOwner!.Value.UserId);

                        // 1 Send  approval email to the user
                        await SendEmail("Your Seat Has Been Automatically Approved", booking.User?.FirstName + booking.User?.LastName, "Your seat request has been automatically approved by the system.", "Approved", booking, "You may view or cancel your booking from your Booking History.");

                        // 2 Send  auto approval email to the seat owner
                        await SendEmail("Your Seat Has Been Automatically Assigned", seatOwner?.FirstName + seatOwner?.LastName, "Your seat has been automatically reassigned to another user.", "Approved", booking, "You may view this update in your Booking History.", new List<int> { seatOwner!.UserId });

                        // 3 Send  auto approval email to the admin
                        await SendEmail("Seat Automatically Assigned from Hybrid User","Admin", "A seat previously assigned to a hybrid user has been automatically reassigned.", "Approved", booking, "You may view this assignment from the Booking History.",admins);
                    }
                    else
                    {
                        
                        // 1 Send  approval email to the user
                        await SendEmail("Your Seat Has Been Automatically Approved", booking.User?.FirstName + booking.User?.LastName, "Your seat request has been automatically approved by the system.", "Approved", booking, "You may view or cancel your booking from your Booking History.");

                        // 2 Send  auto approval email to the admin
                        await SendEmail("Seat Automatically Assigned to User","Admin", "A seat has been automatically assigned to a requesting user by the system.", "Approved", booking, "You may view this update in the Booking History.",admins);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while sending auto approval email to user.", ex);
                return false;
            }
            _logger.Info("Auto Email For Approval completed...");
            return true;


           
    }

    private async Task<bool> SendRejectionEmail(List<Booking> bookingsToApproved, List<Booking?> bookingsToReject )
    {
        try
        {
            //sending email of rejection 
            foreach (var booking in bookingsToReject)
            {
                bool isUserInApprovedList = bookingsToApproved.Any(b => b.UserId == booking?.UserId && b.BookingDate == booking.BookingDate);
                if (isUserInApprovedList)
                {
                    // 1 Send rejection email to the user for other booking
                    await SendEmail("Your Other Seat Request Was Rejected", booking?.User?.FirstName + booking?.User?.LastName, "Your other pending seat request has been automatically rejected.", "Rejected", booking, "You may choose a new seat from the Home Page.");
                }
                else
                {
                    // 2 Send rejection email to all other users
                    await SendEmail("Your Seat Has Been Automatically Rejected", booking?.User?.FirstName + booking?.User?.LastName, "Your seat request has been automatically rejected by the system.", "Rejected", booking, "You may view rejected booking from your Booking History.");
                }
               
            }
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while sending auto rejection email to user.", ex);
            return false;
        }
        _logger.Info("Auto Email For Rejection completed...");
        return true;
    }

}
