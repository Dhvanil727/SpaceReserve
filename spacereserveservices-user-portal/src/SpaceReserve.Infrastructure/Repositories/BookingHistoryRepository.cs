using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Infrastructure.Extensions;

namespace SpaceReserve.Infrastructure.Repositories;

public class BookingHistoryRepository : IBookingHistoryRepository
{
    private readonly AppDbContext _context;

    public BookingHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.Bookings
        .Include(b => b.User)
        .Include(b => b.Seat)
            .ThenInclude(s => s!.ColumnModel)
                .ThenInclude(c => c!.FloorModel)
                    .ThenInclude(f => f!.CityModel)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.DeletedDate == null);
    }

    public async Task UpdateBookingAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetSeatOwnerByBookingIdAsync(int seatId)
    {
        var seatOwner = await _context.SeatConfigurations
                .Include(sc => sc.User)
                .Include(sc => sc.Seat)
                    .ThenInclude(s => s!.ColumnModel)
                    .ThenInclude(c => c!.FloorModel)
                .AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.SeatId == seatId && sc.DeletedDate == null);
        return seatOwner.User;
    }

    public async Task<User?> GetAdminIdAsync()
    {
        return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IsAdmin == true);
    }

    public async Task<List<Booking>> GetAllBookingHistoryAsync(int pageNo, int pageSize, string subjectId)
    {
        return await _context.Bookings
                       .AsNoTracking()
                       .Include(b => b.User)
                       .Include(b => b.BookingStatusModel)
                       .Include(b => b.Seat)
                       .ThenInclude(s => s!.ColumnModel)
                       .ThenInclude(c => c!.FloorModel)
                       .Where(b => b.DeletedDate == null && b.User!.SubjectId == subjectId)
                       .OrderByDescending(b => b.BookingDate)
                       .ThenByDescending(b => b.CreatedDate)
                       .GetPaginated(pageNo, pageSize)
                       .ToListAsync();
    }

    public async Task<List<Booking>> GetAllBookingHistoryByStatusAsync(int? sort, int pageNo, int pageSize, string subjectId)
    {
        return await _context.Bookings
                      .AsNoTracking()
                      .Include(b => b.User)
                      .Include(b => b.BookingStatusModel)
                      .Include(b => b.Seat)
                      .ThenInclude(s => s!.ColumnModel)
                      .ThenInclude(c => c!.FloorModel)
                      .Where(b => b.DeletedDate == null && b.User!.SubjectId == subjectId && b.BookingStatusId == sort)
                      .OrderByDescending(b => b.BookingDate)
                       .ThenByDescending(b => b.CreatedDate)
                      .GetPaginated(pageNo, pageSize)
                      .ToListAsync();
    }

    public async Task<Booking> GetRequestHistoryByIdAsync(int requestId)
    {
        var booking = await _context.Bookings
                                    .Include(b => b.User)
                                    .Include(c => c!.Seat)
                                    .ThenInclude(c => c!.ColumnModel)
                                    .ThenInclude(c => c!.FloorModel)
                                    .ThenInclude(c => c!.CityModel)
                                    .Where(b => b.DeletedDate == null)
                                    .FirstOrDefaultAsync(b => b.BookingId == requestId);
        return booking!;
    }
}