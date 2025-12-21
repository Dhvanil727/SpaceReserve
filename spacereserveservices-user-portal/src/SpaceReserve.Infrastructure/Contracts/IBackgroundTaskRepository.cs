using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface IBackgroundTaskRepository
{
    Task<IEnumerable<Booking>?> GetPendingBookings();

    Task<bool> AutoApprovePendingBookings(IEnumerable<Booking> bookings);
    Task<bool> AutoRejectPendingBookings(IEnumerable<Booking> bookings);

    Task<User?> GetUserById(int userId);

    Task<List<int>> GetAdminsId();

   Task<List<(short SeatId , int UserId)>?> GetSeatOwnersBySeatId(List<short> seatIds);
}
