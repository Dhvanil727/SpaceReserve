using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface IBookingHistoryRepository
{
    Task<Booking?> GetBookingByIdAsync(int bookingId);
    Task UpdateBookingAsync(Booking booking);
    Task<User> GetSeatOwnerByBookingIdAsync(int seatId);
    Task<List<Booking>> GetAllBookingHistoryAsync(int pageNo, int pageSize, string subjectId);
    Task<List<Booking>> GetAllBookingHistoryByStatusAsync(int? sort, int pageNo, int pageSize, string subjectId);
    Task<Booking> GetRequestHistoryByIdAsync(int requestId);
    Task<User> GetAdminIdAsync();

}
