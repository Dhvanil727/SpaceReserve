using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface IRequestHistoryRepository
{
    Task<Booking> GetBookingByIdAsync(int bookingId);
    Task<User> GetUserBySubjectIdAsync(string subjectId);
    Task<bool> UpdateUserRequestStatusAsync(Booking booking);
    Task<List<Booking>> GetAllBookingsOfUserAndSeatAsync(int userId, int seatId, DateOnly date);
    Task<SeatConfiguration?> GetSeatOwnerByIdAsync(int seatId);
    Task<IEnumerable<BookingStatusModel>> GetRequestStatusesAsync();
    Task<IEnumerable<Booking>?> GetAllRequestHistory(int seatId, int pageNumber, int pageSize);
    Task<IEnumerable<Booking>?> GetAllRequestHistoryByStatus(int seatId, int status, int pageNumber, int pageSize);
    Task<User?> GetBySubjectIdAsync(string subjectId);
    Task<int> GetUserSeatID(int userId);
    Task<List<int>> GetAdminsId();
}

