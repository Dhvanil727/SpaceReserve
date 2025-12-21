using System.Reflection;
using log4net;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.AppService.Services;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Admin.Utility.Resources;
using Hangfire;

namespace SpaceReserve.Admin.AppService.Services;

public class RequestHistoryService : IRequestHistoryService
{
    private readonly IRequestHistoryRepository _requestHistoryRepository;
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(RequestHistoryService));
    private readonly IEmailService _emailService;

    public RequestHistoryService(IRequestHistoryRepository requestHistoryRepository, IEmailService emailService)
    {
        _requestHistoryRepository = requestHistoryRepository;
        _emailService = emailService;

    }

    public async Task<bool> UpdateUserRequestStatusAsync(int requestId, byte statusId, string subjectId)
    {
        var admin = await _requestHistoryRepository.GetAdminIdAsync(subjectId);
        if (admin == null)
        {
            throw new ArgumentException(CommonResources.AdminNotFound);
        }
        var bookingByRequestId = await _requestHistoryRepository.GetUserRequestByIdAsync(requestId);
        if (bookingByRequestId == null)
        {
            throw new ArgumentException("Booking doesn't exist or already modified");
        }
        var bookingDate = bookingByRequestId.BookingDate;
        var bookingSeatId = bookingByRequestId.SeatId;
        var userId = bookingByRequestId.UserId;


        var bookingsOfUserAndSeat = await _requestHistoryRepository.GetAllBookingOfUserAndSeatAsync(userId, bookingSeatId, bookingDate);
        if (bookingsOfUserAndSeat == null)
        {
            throw new ArgumentException("Bookings not found");
        }
        var seatOwner = await _requestHistoryRepository.GetSeatOwnerByIdAsync(bookingSeatId);
        if (seatOwner != null)
        {
            throw new ArgumentException("This booking doesn't belong to admin");
        }
        bookingByRequestId.BookingStatusId = statusId;
        bookingByRequestId.ModifiedBy = admin!.UserId;
        bookingByRequestId.ModifiedDate = DateTime.Now;
        var isUpdate = await _requestHistoryRepository.UpdateUserRequestStatusAsync(bookingByRequestId);
        // var admin = await _requestHistoryRepository.GetAdminUserAsync();

        if (statusId == (byte)CommonResources.BookingStatus.Rejected && isUpdate)
        {
            await SendEmail(EmailHelper.SeatRequestRejectSubject, bookingByRequestId?.User?.FirstName, EmailHelper.SeatRequestRejectStatus, EmailHelper.Rejected, admin.FirstName, bookingByRequestId, EmailHelper.SeatRequestRejectAutoMessage, bookingByRequestId!.UserId);
        }
        else if (statusId == (byte)CommonResources.BookingStatus.Accepted && isUpdate)
        {
            await SendEmail(EmailHelper.SeatRequestApproveSubject, bookingByRequestId?.User?.FirstName, EmailHelper.SeatRequestApproveStatus, EmailHelper.Approved, admin.FirstName, bookingByRequestId, EmailHelper.SeatRequestApproveAutoMessage, bookingByRequestId!.UserId);
        }
        if (!isUpdate)
        {
            throw new ArgumentException("failed to update the status of requested bookingId");
        }
        if (statusId == (byte)CommonResources.BookingStatus.Accepted)
        {
            foreach (var booking in bookingsOfUserAndSeat)
            {
                if (booking.BookingId != requestId)
                {
                    booking.BookingStatusId = (byte)CommonResources.BookingStatus.Rejected;
                    booking.ModifiedBy = admin!.UserId;
                    booking.ModifiedDate = DateTime.Now;
                    var isUserUpdate = await _requestHistoryRepository.UpdateUserRequestStatusAsync(booking);
                    if (!isUserUpdate)
                    {
                        throw new ArgumentException("failed to update the status of requested other bookingId");
                    }
                    if (booking.UserId == bookingByRequestId.UserId)
                    {
                        await SendEmail(EmailHelper.UserOtherSeatsRequestRejectSubject, booking.User?.FirstName, EmailHelper.UserOtherSeatsRequestRejectStatus, EmailHelper.AutoReject, "", booking, EmailHelper.UserOtherSeatsRequestRejectAutoMessage, booking.UserId);
                    }
                    if (booking.SeatId == bookingByRequestId.SeatId)
                    {
                        await SendEmail(EmailHelper.OtherUsersRequestOfSeatRejectSubject, booking.User?.FirstName, EmailHelper.OtherUsersRequestOfSeatRejectStatus, EmailHelper.Rejected, admin.FirstName, booking, EmailHelper.OtherUsersRequestOfSeatRejectAutoMessage, booking.UserId);
                    }
                }
            }
        }
        return true;
    }

    public async Task<bool> SendEmail(string? subject, string? dear, string? heading, string? status, string? autoMessage, Booking? booking, string? endMessage, int userId)
    {
        var emailDto = new EmailDto
        {
            Subject = subject!,
            Body = EmailHelper.GenerateSeatRequestEmailBody(
                       dear!,
                       heading!,
                       status!,
                       autoMessage!,
                       booking!.BookingDate,
                       booking.Seat?.ColumnModel?.FloorModel?.CityModel?.City,
                       booking.Seat?.ColumnModel?.FloorModel?.Floor,
                       booking.Seat?.ColumnModel!.Column + "" + booking!.Seat!.SeatNumber,
                       endMessage!
                         ),
            ToUserId = [userId,],
        };
        try
        {
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(emailDto));
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while sending auto proccess email to user.", ex);
            return false;
        }
        return true;
    }

    public async Task<List<RequestHistoryDto>> GetRequestHistoryAsync(int? sort, int pageNo, int pageSize)
    {

        var history = await _requestHistoryRepository.GetRequestHistoryAsync(sort, pageNo, pageSize);
        var requestHistoryDtos = history.Select(h => new RequestHistoryDto
        {
            RequestId = h.BookingId,
            FullName = h.User != null ? h.User.FirstName + " " + h.User.LastName : CommonResources.NotAvailable,
            Email = h.User?.Email ?? CommonResources.NotAvailable,
            RequestDate = h.CreatedDate.ToString("MM/dd/yyyy"),
            BookingDate = h.BookingDate.ToString("MM/dd/yyyy"),
            FloorNo = h.Seat?.ColumnModel?.FloorModel?.Floor ?? "",
            DeskNumber = h.Seat != null ? h.Seat.ColumnModel?.Column + "" + h.Seat.SeatNumber : CommonResources.NotAvailable,
            Status = h?.BookingStatusId ?? 0
        }).ToList();

        return requestHistoryDtos;
    }

    public async Task<List<BookingStatusDto>> GetRequestStatusDropdwon()
    {
        var requestStatus = await _requestHistoryRepository.GetRequestStatusDropdwon();
        var requestStatusDtos = requestStatus.Select(s => new BookingStatusDto
        {
            Id = s.BookingStatusId,
            BookingStatus = s.BookingStatus
        }).ToList();
        return requestStatusDtos;

    }

    public async Task<GetSingleRequestHistoryDto> GetRequestHistoryByIdAsync(int requestId)
    {
        var booking = await _requestHistoryRepository.GetRequestHistoryByIdAsync(requestId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found");
        }
        var getSingleRequestHistoryDto = new GetSingleRequestHistoryDto
        {
            RequestId = booking.BookingId,
            FullName = booking.User != null ? booking.User.FirstName + " " + booking.User.LastName : CommonResources.NotAvailable,
            Email = booking.User?.Email ?? CommonResources.NotAvailable,
            RequestDate = booking.CreatedDate.ToString("MM/dd/yyyy"),
            RequestedFor = booking.BookingDate.ToString("MM/dd/yyyy"),
            FloorNo = booking.Seat?.ColumnModel?.FloorModel?.Floor ?? "",
            DeskNo = booking.Seat != null ? booking.Seat.ColumnModel?.Column + "" + booking.Seat.SeatNumber : CommonResources.NotAvailable,
            Status = booking?.BookingStatusId ?? 0,
            Reason = booking?.Reason ?? CommonResources.NotAvailable
        };
        return getSingleRequestHistoryDto;
    }
}
