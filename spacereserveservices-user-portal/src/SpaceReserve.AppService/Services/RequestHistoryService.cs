using AutoMapper;
using Hangfire;
using Hangfire.States;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.AppService.DTOs.RequestHistory;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Enum;
using static SpaceReserve.Utility.Resources.CommonResource;
using static SpaceReserve.Utility.Resources.EmailResources;
using static SpaceReserve.Utility.Resources.EmailTemplate;

namespace SpaceReserve.AppService.Services;

public class RequestHistoryService : IRequestHistoryService
{
    private readonly IRequestHistoryRepository _requestHistoryRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    public RequestHistoryService(IRequestHistoryRepository requestHistoryRepository, IMapper mapper, IEmailService emailService)
    {
        _requestHistoryRepository = requestHistoryRepository;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<bool> UpdateUserRequestStatusAsync(int requestId, byte statusId, string subjectId)
    {
        var loggedInUser = await _requestHistoryRepository.GetUserBySubjectIdAsync(subjectId);
        if (loggedInUser == null)
        {
            throw new ArgumentException("Logged in user not found.");
        }
        var bookingByRequestId = await _requestHistoryRepository.GetBookingByIdAsync(requestId);
        if (bookingByRequestId == null)
        {
            throw new ArgumentException("Booking already modified or doesn't exists.");
        }
        var bookingDate = bookingByRequestId.BookingDate;
        var seatId = bookingByRequestId.SeatId;
        var userId = bookingByRequestId.UserId;
        var bookingsOfUserAndSeat = await _requestHistoryRepository.GetAllBookingsOfUserAndSeatAsync(userId, seatId, bookingDate);
        if (bookingsOfUserAndSeat == null)
        {
            throw new ArgumentException("Bookings not found");
        }
        var seatOwner = await _requestHistoryRepository.GetSeatOwnerByIdAsync(seatId);
        if(seatOwner==null){
            throw new ArgumentException("This booking request doesn't belong to loggedIn user.");
        }
        if (seatOwner!.UserId != loggedInUser.UserId)
        {
            throw new ArgumentException(NotAnOwner);
        }
        var admins = await _requestHistoryRepository.GetAdminsId();
        bookingByRequestId.BookingStatusId = statusId;
        bookingByRequestId.ModifiedBy = loggedInUser.UserId;
        bookingByRequestId.ModifiedDate = DateTime.Now;
        var isUpdated = await _requestHistoryRepository.UpdateUserRequestStatusAsync(bookingByRequestId);
        if (!isUpdated)
        {
            throw new ArgumentException("Failed to update the status of requested bookingId");
        }
        if (statusId == (byte)BookingStatus.Rejected && isUpdated)
        {
            await SendEmail(SeatRequestRejectedSubject, bookingByRequestId.User!.FirstName, SeatRequestRejectHeading,Rejected, loggedInUser.FirstName + " " +loggedInUser.LastName , bookingByRequestId, SeatRequestRejectedEndMessage);
            await SendEmail(SeatRequestRejectedSubjectAdmin, Admin, SeatRequestRejectHeadingAdmin,Rejected, loggedInUser.FirstName + " " +loggedInUser.LastName , bookingByRequestId, SeatRequestRejectedEndMessageAdmin, admins);     
        }
        else if (statusId == (byte)BookingStatus.Accepted  && isUpdated)
        {
            await SendEmail(SeatApprovedSubject, bookingByRequestId.User!.FirstName,SeatApprovedHeading ,Approved, loggedInUser.FirstName + " " +loggedInUser.LastName , bookingByRequestId, SeatApprovedEndMessage);
            await SendEmail(SeatApprovedSubjectAdmin, Admin, SeatApprovedHeadingAdmin, Approved,loggedInUser.FirstName + " " +loggedInUser.LastName , bookingByRequestId, SeatApprovedEndMessageAdmin, admins);
        }
        if (statusId == (byte)BookingStatus.Accepted)
        {
            foreach (var booking in bookingsOfUserAndSeat)
            {
                if (booking.BookingId != requestId)
                {
                    booking.BookingStatusId = (byte)BookingStatus.Rejected;
                    booking.ModifiedBy = loggedInUser.UserId;
                    booking.ModifiedDate = DateTime.Now;
                    var isSeatsUpdated = await _requestHistoryRepository.UpdateUserRequestStatusAsync(booking);
                    if (!isSeatsUpdated)
                    {
                        throw new ArgumentException("Failed to update the status of other bookingIds");
                    }
                    if(booking.UserId==userId){
                      await SendEmail(OtherSeatRequestRejectedSubject, booking.User!.FirstName, SeatRequestRejectedAutoHeading,"",AutoRejected, booking, SeatRequestRejectedAutoEndMessage);
                
                     await SendEmail(SeatRequestRejectedSubjectAdmin, Admin,SeatRequestRejectHeadingAdmin,Rejected, loggedInUser.FirstName + " " +loggedInUser.LastName , booking, SeatRequestRejectedEndMessageAdmin, admins);
                    }
                     if(booking.SeatId==seatId){
                         await SendEmail(SeatRequestRejectedSubject, booking.User!.FirstName, SeatRequestRejectHeading,Rejected, loggedInUser.FirstName + " " +loggedInUser.LastName , booking, SeatRequestRejectedEndMessage);

                         await SendEmail(SeatRequestRejectedSubjectAdmin, Admin, SeatRequestRejectHeadingAdmin, Rejected,loggedInUser.FirstName + " " +loggedInUser.LastName , booking, SeatRequestRejectedEndMessageAdmin, admins);
                    }
                }
            }
        }
        return true;
    }

    public async Task<IEnumerable<BookingStatusModel>> GetRequestStatusesAsync()
    {
        return await _requestHistoryRepository.GetRequestStatusesAsync();
    }

    public async Task<IEnumerable<RequestHistoryDTO>?> GetAllRequestHistory(int seatId, int pageNo, int pageSize)
    {

        var requestHistory = await _requestHistoryRepository.GetAllRequestHistory(seatId, pageNo, pageSize);
        return _mapper.Map<IEnumerable<RequestHistoryDTO>>(requestHistory);
    }

    public async Task<IEnumerable<RequestHistoryDTO>?> GetAllRequestHistoryByStatus(int seatId, int status, int pageNo, int pageSize)
    {

        var requestHistory = await _requestHistoryRepository.GetAllRequestHistoryByStatus(seatId, status, pageNo, pageSize);
        return _mapper.Map<IEnumerable<RequestHistoryDTO>>(requestHistory);
    }

    public async Task<User?> GetBySubjectIdAsync(string subjectId)
    {
        return await _requestHistoryRepository.GetBySubjectIdAsync(subjectId);
    }

    public Task<int> GetUserSeatID(int userId)
    {
        return _requestHistoryRepository.GetUserSeatID(userId);
    }

    private async Task<bool> SendEmail(string subject, string dear, string heading, string status, string request, Booking booking, string endMessage, List<int>? adminTo = null)
    {

        EmailDto emailDto = new EmailDto
        {
            Subject = $"{subject}",
            Body = GetEmailTemplate(
                dear,
                heading,
                status,
                request,
                booking.BookingDate,
                booking.Seat!.ColumnModel!.FloorModel!.CityModel!.City,
                booking.Seat!.ColumnModel!.FloorModel.Floor,
                booking.Seat?.ColumnModel!.Column + "" + booking!.Seat!.SeatNumber,
                endMessage),
            ToUserId = adminTo?.Count > 0 ? adminTo : new List<int> { booking.UserId },
        };
        try
        {
            BackgroundJobClient client = new BackgroundJobClient();
            client.Create(() => _emailService.SendEmailAsync(emailDto), new EnqueuedState("user-side"));
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}
