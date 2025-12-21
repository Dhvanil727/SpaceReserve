using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Contracts
{
    public interface IBookingHistoryRepository
    {
        Task<List<Booking>> GetAllBookingHistoryAsync(int pageNo, int pageSize);
        Task<List<Booking>> GetUpcomingBookingHistoryAsync(int pageNo, int pageSize);
        Task<List<Booking>> GetPastBookingHistoryAsync(int pageNo, int pageSize);
    }
}