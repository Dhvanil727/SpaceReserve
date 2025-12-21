using AutoMapper;
using Microsoft.AspNetCore.Http;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.AppService.DTOs.RequestHistory;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Enum;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.AppService.Services;

public class BookingHistoryService : IBookingHistoryService
{
    private readonly IBookingHistoryRepository _bookingHistoryRepository;
    private readonly IBackgroundTaskRepository _backgroundTaskRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public BookingHistoryService(IBookingHistoryRepository bookingHistoryRepository, IMapper mapper, IEmailService emailService, IBackgroundTaskRepository backgroundTaskRepository)
    {
        _backgroundTaskRepository = backgroundTaskRepository;
        _bookingHistoryRepository = bookingHistoryRepository;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<bool> CancelBookingAsync(int bookingId)
    {
        var booking = await _bookingHistoryRepository.GetBookingByIdAsync(bookingId);
        if (booking == null)
        {
            return false;
        }
        var admin = await _bookingHistoryRepository.GetAdminIdAsync();//foe admin Id
        if (booking.BookingStatusId == (byte)BookingStatus.Pending || booking.BookingStatusId == (byte)BookingStatus.Accepted)
        {
            var seatOwners = await GetSeatOwnersBySeatId(new List<short> { booking.SeatId });
            if (booking.BookingStatusId == (byte)BookingStatus.Pending)
            {
                booking.BookingStatusId = (byte)BookingStatus.Cancelled;
                booking.ModifiedBy = booking.UserId;
                booking.ModifiedDate = DateTime.Now;
                await _bookingHistoryRepository.UpdateBookingAsync(booking);

                if (seatOwners != null && seatOwners.Count > 0)
                {
                    var ownerDetails = await _backgroundTaskRepository.GetUserById(seatOwners[0].UserId);
                    await SendEmail(CancelRequestEmailResource.RequestCancelByUser, $"{ownerDetails?.FirstName} {ownerDetails?.LastName}", CancelRequestEmailResource.SeatRequestCancelHeading, booking, CancelRequestEmailResource.SeatRequestCancelEndMessage, ownerDetails?.UserId ?? 0);
                }
                else
                {
                    await SendEmail(CancelRequestEmailResource.RequestCancelByUser, CommonResource.Admin, CancelRequestEmailResource.SeatRequestCancelHeadingAdmin, booking, CancelRequestEmailResource.SeatRequestCancelEndMessageAdmin, admin.UserId);
                }
            }
            else
            {
                if (booking.BookingStatusId == (byte)BookingStatus.Accepted && (booking.BookingDate.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).TotalHours <= 24)
                {
                    return false;
                }
                else
                {
                    booking.BookingStatusId = (byte)BookingStatus.Cancelled;
                    booking.ModifiedBy = booking.UserId;
                    booking.ModifiedDate = DateTime.Now;
                    await _bookingHistoryRepository.UpdateBookingAsync(booking);

                    if (seatOwners != null && seatOwners.Count > 0)
                    {
                        var ownerDetails = await _backgroundTaskRepository.GetUserById(seatOwners[0].UserId);
                        await SendEmail(CancelRequestEmailResource.ReservationCancelByUser, $"{ownerDetails?.FirstName} {ownerDetails?.LastName}", CancelRequestEmailResource.SeatReservationCancelHeading, booking, CancelRequestEmailResource.SeatReservationCancelEndMessage, ownerDetails?.UserId ?? 0);
                    }
                    else
                    {
                        await SendEmail(CancelRequestEmailResource.ReservationCancelByUser, CommonResource.Admin, CancelRequestEmailResource.SeatReservationCancelHeadingAdmin, booking, CancelRequestEmailResource.SeatReservationCancelEndMessageAdmin, admin.UserId);
                    }
                }
            }
            return true;
        }
        return false;
    }

    public async Task<List<BookingHistoryDto>> GetAllBookingHistoriesAsync(int sort, int pageNo, int pageSize, string subjectId)
    {
        if (sort >= (int)BookingStatus.Pending && sort <= (int)BookingStatus.Cancelled)
        {
            var bookingHistoryBystatus = await _bookingHistoryRepository.GetAllBookingHistoryByStatusAsync(sort, pageNo, pageSize, subjectId);
            var bookingHistoryDto = _mapper.Map<List<BookingHistoryDto>>(bookingHistoryBystatus);
            return bookingHistoryDto;
        }
        else
        {
            var allBookingHistory =await _bookingHistoryRepository.GetAllBookingHistoryAsync(pageNo,pageSize,subjectId);
            var bookingHistoryDto = _mapper.Map<List<BookingHistoryDto>>(allBookingHistory);
            return bookingHistoryDto;
        }
    }

    private async Task<List<(short SeatId, int UserId)>?> GetSeatOwnersBySeatId(List<short> seatIds)
    {
        var seatOwners = await _backgroundTaskRepository.GetSeatOwnersBySeatId(seatIds);
        return seatOwners;
    }

    private async Task<bool> SendEmail(string subject, string dear, string heading, Booking booking, string endMessage, int toUserId)
    {
        EmailDto emailDto = new EmailDto
        {
            Subject = $"{subject}",
            Body = EmailTemplate.CancelBookingEmail(
                dear,
                heading,
                $"{booking.User!.FirstName} {booking.User!.LastName}",
                booking.BookingDate,
                booking.Seat!.ColumnModel!.FloorModel!.CityModel!.City,
                booking.Seat!.ColumnModel!.FloorModel.Floor,
                $"{booking.Seat!.ColumnModel!.Column}{booking.Seat!.SeatNumber}",
                endMessage
            ),
            ToUserId = new List<int> { toUserId },
            SenderSubjectId = booking.User!.SubjectId,
        };
        try
        {
            await _emailService.SendEmailAsync(emailDto);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}