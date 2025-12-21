using SpaceReserve.AppService.DTOs;
using SpaceReserve.AppService.DTOs.RequestHistory;

namespace SpaceReserve.AppService.Contracts;

public interface IBookingHistoryService
{
    Task<bool> CancelBookingAsync(int bookingId);
    Task<List<BookingHistoryDto>> GetAllBookingHistoriesAsync(int sort, int pageNo, int pageSize,string subjectId);
}