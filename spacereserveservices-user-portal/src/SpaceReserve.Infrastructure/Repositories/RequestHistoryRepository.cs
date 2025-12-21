using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Infrastructure.Extensions;

namespace SpaceReserve.Infrastructure.Repositories;

public class RequestHistoryRepository : IRequestHistoryRepository
{
    private readonly AppDbContext _context;
    public RequestHistoryRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Booking>> GetAllBookingsOfUserAndSeatAsync(int userId, int seatId, DateOnly date)
    {
        var bookings = await _context.Bookings
       .Include(b => b.User)
       .Include(c => c.Seat)
       .ThenInclude(c => c.ColumnModel)
       .ThenInclude(c => c.FloorModel)
       .ThenInclude(c => c.CityModel)
       .Where(b => (b.UserId == userId || b.SeatId == seatId) && b.BookingDate == date && b.ModifiedBy == null && b.DeletedBy == null)
       .ToListAsync();
        return bookings;
    }
    public async Task<Booking> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _context.Bookings
                             .Include(b => b.User)
                             .Include(c => c.Seat)
                             .ThenInclude(c => c.ColumnModel)
                             .ThenInclude(c => c.FloorModel)
                             .ThenInclude(c => c.CityModel)
                             .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.ModifiedBy == null && b.DeletedBy == null);
        return booking!;
    }

    public async Task<SeatConfiguration?> GetSeatOwnerByIdAsync(int seatId)
    {
        var owner = await _context.SeatConfigurations
             .AsNoTracking()
             .Where(sc => sc.SeatId == seatId && sc.DeletedDate == null)
             .FirstOrDefaultAsync();
        return owner;
    }

    public async Task<User> GetUserBySubjectIdAsync(string subjectId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.SubjectId == subjectId);
        return user!;
    }
    public async Task<bool> UpdateUserRequestStatusAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<BookingStatusModel>> GetRequestStatusesAsync()
    {
        return await _context.Set<BookingStatusModel>()
             .OrderBy(status => status.BookingStatusId)
             .Select(status => new BookingStatusModel
             {
                 BookingStatusId = status.BookingStatusId,
                 BookingStatus = status.BookingStatus
             })
             .ToListAsync();
    }

    public async Task<IEnumerable<Booking>?> GetAllRequestHistory(int seatId, int pageNo, int pageSize)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.SeatId == seatId && b.Type == "Request" && b.DeletedBy == null)
            .Include(b => b.User)
            .Include(b => b.BookingStatusModel)
            .Include(b => b.Seat)
                .ThenInclude(s => s.ColumnModel)
                .ThenInclude(c => c.FloorModel)
            .OrderByDescending(b => b.CreatedDate)
            .GetPaginated(pageNo, pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>?> GetAllRequestHistoryByStatus(int seatId, int status, int pageNo, int pageSize)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.SeatId == seatId && b.BookingStatusId == status && b.Type == "Request" && b.DeletedBy == null)
            .Include(b => b.BookingStatusModel)
            .Include(b => b.User)
            .Include(b => b.Seat)
                .ThenInclude(s => s.ColumnModel)
                .ThenInclude(c => c.FloorModel)
            .OrderByDescending(b => b.CreatedDate)
            .GetPaginated(pageNo, pageSize)
            .ToListAsync();
    }

    public async Task<User?> GetBySubjectIdAsync(string subjectId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.SubjectId == subjectId);
    }

    public async Task<int> GetUserSeatID(int userId)
    {
        return await _context.SeatConfigurations.Where(s => s.UserId == userId  && s.DeletedDate == null ).Select(s => s.SeatId).FirstOrDefaultAsync();
    }
    public async Task<List<int>> GetAdminsId()
    {
        return await _context.Users
                        .Where(u => u.IsAdmin == true && u.DeletedDate == null)
                        .Select(u => u.UserId)
                        .ToListAsync();
    }
}
