using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.Admin.Infrastructure.Extensions;
using SpaceReserve.Admin.Utility.Resources;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Repositories;

public class RequestHistoryRepository : IRequestHistoryRepository
{
    private readonly AppDbContext _context;

    public RequestHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllBookingOfUserAndSeatAsync(int userId, int seatId, DateOnly date)
    {
        return await _context.Bookings
                        .Include(b => b.User)
                        .Include(c => c!.Seat)
                            .ThenInclude(c => c!.ColumnModel)
                                .ThenInclude(c => c!.FloorModel)
                                    .ThenInclude(c => c!.CityModel)
                     .Where(b => (b.UserId == userId || b.SeatId == seatId) && b.BookingDate == date && b.ModifiedBy == null && b.DeletedDate == null)
                     .ToListAsync();
    }

    public async Task<Booking> GetUserRequestByIdAsync(int requestId)
    {
        var booking = await _context.Bookings
                        .Include(b => b.User)
                        .Include(c => c!.Seat)
                            .ThenInclude(c => c!.ColumnModel)
                                .ThenInclude(c => c!.FloorModel)
                                .ThenInclude(c => c!.CityModel)
                     .FirstOrDefaultAsync(b => b.BookingId == requestId && b.ModifiedBy == null && b.DeletedDate == null);
        return booking!;
    }
    public async Task<bool> UpdateUserRequestStatusAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<Booking>> GetRequestHistoryAsync(int? sort, int pageNo, int pageSize)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(c => c!.Seat)
                .ThenInclude(c => c!.ColumnModel)
                    .ThenInclude(c => c!.FloorModel)
                        .ThenInclude(c => c!.CityModel)
            .Where(b => b.Type == CommonResources.BookingTypeForUnAssignedSeat && ((sort == null || sort == Convert.ToInt32(CommonResources.BookingStatus.All)) ? true : b.BookingStatusId == sort) && b.DeletedDate == null)
            .OrderByDescending(b => b.CreatedDate)
            .GetPaginated(pageNo , pageSize)
            .ToListAsync();
    }

    public async Task<List<BookingStatusModel>> GetRequestStatusDropdwon()
    {
        var requestStatus = await _context.BookingStatus.ToListAsync();
        return requestStatus;
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
    public Task<User> GetAdminUserAsync()
    {
        var adminUser = _context.Users.FirstOrDefaultAsync(u => u.IsAdmin == true && u.DeletedDate == null);
        return adminUser!;
    }

     public async Task<User?> GetAdminIdAsync(string subjectId)
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.IsAdmin == true && u.SubjectId == subjectId);
            return admin;
        }

    public async Task<SeatConfiguration> GetSeatOwnerByIdAsync(int seatId)
    {
        var seatOwner=await _context.SeatConfigurations.Where(s=>s.SeatId==seatId && s.DeletedDate==null).FirstOrDefaultAsync();
        return seatOwner!;
    }
}
