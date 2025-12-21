using SpaceReserve.Admin.AppService.DTOs;

namespace SpaceReserve.Admin.AppService.Contracts;

public interface IBookingHistoryService
{
    Task<List<BookingHistoryDto>> GetBookingHistoryAsync(BookingHistoryQueryDto bookingHistoryQueryDto);
}
