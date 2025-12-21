using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Repositories;

public class BackgroundTaskRepository : IBackgroundTaskRepository
{
    private readonly AppDbContext _context;
    public BackgroundTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AutoApprovePendingBookings(IEnumerable<Booking> bookings)
    {
        _context.Bookings.UpdateRange(bookings);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AutoRejectPendingBookings(IEnumerable<Booking> bookings)
    {

        _context.Bookings.UpdateRange(bookings);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Booking>?> GetPendingBookings()
    {
        var pendingBookings = await _context.Bookings
            .Where(b => b.BookingStatusId == 1 && b.CreatedDate.Date <= DateTime.Now.Date.AddDays(-1) && b.DeletedBy == null)
            .Include(b => b.User)
            .Include(b => b.Seat)
                .ThenInclude(s => s!.ColumnModel)
                    .ThenInclude(c => c!.FloorModel)
                        .ThenInclude(f => f!.CityModel)
            .ToListAsync();

        return pendingBookings;
    }  
    public async Task<List<(short SeatId, int UserId)>?> GetSeatOwnersBySeatId(List<short> seatIds)
    {
        var hybridOwners = await _context.SeatConfigurations
            .Include(b => b.User)
                .ThenInclude(u => u!.ModeOfWorkModel)
            .Where(b => seatIds.Contains(b.SeatId) && b.User != null && b.User.ModeOfWorkId == 1 && b.DeletedBy == null)
            .Select(b => new ValueTuple<short, int>(b.SeatId, b.User!.UserId))
            .ToListAsync();

        return hybridOwners;
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await _context.Users
                        .FirstOrDefaultAsync(u => u.UserId == userId && u.DeletedDate == null);
    }

    public async Task<List<int>> GetAdminsId()
    {
        return await _context.Users
                        .Where(u => u.IsAdmin == true && u.DeletedDate == null)
                        .Select(u => u.UserId)
                        .ToListAsync();
    }
}