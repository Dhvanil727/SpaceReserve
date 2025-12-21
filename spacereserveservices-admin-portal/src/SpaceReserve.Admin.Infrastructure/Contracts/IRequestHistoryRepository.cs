using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Contracts;

public interface IRequestHistoryRepository
{
    Task<Booking> GetUserRequestByIdAsync(int requestId);
    Task<bool> UpdateUserRequestStatusAsync(Booking booking);
    Task<IEnumerable<Booking>> GetAllBookingOfUserAndSeatAsync(int userId, int seatId, DateOnly date);
    public Task<List<Booking>> GetRequestHistoryAsync(int? sort, int pageNo, int pageSize);
    public Task<List<BookingStatusModel>> GetRequestStatusDropdwon();
    public Task<Booking> GetRequestHistoryByIdAsync(int requestId);
    Task<User> GetAdminUserAsync();
    Task<User?> GetAdminIdAsync(string subjectId);
    Task<SeatConfiguration> GetSeatOwnerByIdAsync(int seatId);
}









