using System.Runtime.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.Admin.Infrastructure.Extensions;
using SpaceReserve.Admin.Utility.Resources;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Repositories;

public class RegisteredUserRepository : IRegisteredUserRepository
{
    private readonly AppDbContext _context;

    public RegisteredUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.Bookings.FindAsync(bookingId);
    }
    public async Task<List<Booking>> GetAllBookingHistoryOfUserAsync(int pageNo, int pageSize, int userId)
    {
        return await _context.Bookings
                       .AsNoTracking()
                       .Include(b => b.User)
                       .Include(b => b.BookingStatusModel)
                       .Include(b => b.Seat)
                       .ThenInclude(s => s!.ColumnModel)
                       .ThenInclude(c => c!.FloorModel)
                       .Where(b => b.DeletedDate == null && b.User!.UserId == userId)
                       .OrderByDescending(b => b.BookingDate)
                       .ThenByDescending(b => b.CreatedDate)
                       .GetPaginated(pageNo, pageSize)
                       .ToListAsync();
    }
    public async Task<List<Booking>> GetPastBookingHistoryOfUserAsync(int pageNo, int pageSize, int userId)
    {
        return await _context.Bookings
                       .AsNoTracking()
                       .Include(b => b.User)
                       .Include(b => b.BookingStatusModel)
                       .Include(b => b.Seat)
                       .ThenInclude(s => s!.ColumnModel)
                       .ThenInclude(c => c!.FloorModel)
                       .Where(b => b.BookingDate < DateOnly.FromDateTime(DateTime.Now) && b.BookingStatusId != (int)CommonResources.BookingStatus.Pending && b.DeletedDate == null && b.User!.UserId == userId)
                       .OrderByDescending(b => b.BookingDate)
                       .ThenByDescending(b => b.CreatedDate)
                       .GetPaginated(pageNo, pageSize)
                       .ToListAsync();
    }

    public async Task<List<Booking>> GetUpcomingBookingHistoryOfUserAsync(int pageNo, int pageSize, int userId)
    {
        return await _context.Bookings
                       .AsNoTracking()
                       .Include(b => b.User)
                       .Include(b => b.BookingStatusModel)
                       .Include(b => b.Seat)
                       .ThenInclude(s => s!.ColumnModel)
                       .ThenInclude(c => c!.FloorModel)
                       .Where(b => b.BookingDate >= DateOnly.FromDateTime(DateTime.Now) && b.BookingDate <= DateOnly.FromDateTime(DateTime.Now).AddMonths(3) && b.DeletedBy == null && b.User!.UserId == userId)
                       .OrderBy(b => b.BookingDate)
                       .ThenBy(b => b.CreatedDate)
                       .GetPaginated(pageNo, pageSize)
                       .ToListAsync();
    }


    public async Task<User?> GetUserProfileBySubjectIdAsync(int userId)
    {
        return await _context.Users
                            .AsNoTracking()
                            .AsSplitQuery()
                            .Include(u => u.ModeOfWorkModel)
                            .Include(u => u.DesignationModel)
                            .Include(u => u.CityModel)
                            .Include(u => u.UserWorkingDays)
                            .ThenInclude(uwd => uwd.WorkingDay)
                            .Where(u => u.UserId == userId)
                            .FirstOrDefaultAsync();
    }
    public async Task<SeatConfiguration?> GetSeatConfigurationByUserIdAsync(int userId)
    {
        return await _context.SeatConfigurations.FirstOrDefaultAsync(sc => sc.UserId == userId && sc.DeletedDate == null);
    }

    public async Task<Seat?> GetSeatByIdAsync(short seatId)
    {
        return await _context.Seats.FirstOrDefaultAsync(s => s.SeatId == seatId);
    }

    public async Task<ColumnModel?> GetColumnByIdAsync(short columnId)
    {
        return await _context.ColumnModels.FirstOrDefaultAsync(c => c.ColumnId == columnId);
    }

    public async Task<FloorModel?> GetFloorByIdAsync(short floorId)
    {
        return await _context.FloorModels.FirstOrDefaultAsync(c => c.FloorId == floorId);
    }

    public async Task<List<short>> GetAllAssignedSeatIdsAsync()
    {
        return await _context.SeatConfigurations
                .AsNoTracking()
                .Select(sc => sc.SeatId)
                .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(int pageNo, int pageSize)
    {
        return await _context.Users.Where(u => u.IsAdmin == false)
            .AsNoTracking()
            .Include(u => u.DesignationModel)
            .GetPaginated(pageNo, pageSize)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task<List<User>> GetUserByUserId(int userId)
    {
        return await _context.Users.Where(b => b.UserId == userId).ToListAsync();
    }

    public async Task<List<Booking>> GetBookingsByUserId(int userId)
    {
        return await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
    }

    public async Task<List<SeatConfiguration>> GetSeatConfigurationsByUserId(int userId)
    {
        return await _context.SeatConfigurations.Where(sc => sc.UserId == userId).ToListAsync();
    }

    public async Task<List<UserWorkingDay>> GetUserWorkingDaysByUserId(int userId)
    {
        return await _context.UserWorkingDays.Where(uw => uw.UserId == userId).ToListAsync();
    }
    public async Task<SeatConfiguration> GetSeatIdOfUserAsync(int userId)
    {
        var seat = await _context.SeatConfigurations.Where(sc => sc.UserId == userId && sc.DeletedDate == null).FirstOrDefaultAsync();
        return seat;
    }
    public async Task<List<Booking>> GetAllBookingsOfSeatIdAsync(int seatId)
    {
        var bookings = await _context.Bookings.Where(b => b.SeatId == seatId && b.DeletedDate == null && b.BookingStatusId != (byte)CommonResources.BookingStatus.Rejected && b.BookingDate >= DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
        return bookings;
    }
    public async Task<IDbContextTransaction> BeginTransactionAndRollbackAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<User?> GetAdminBySubjectIdAsync(string subjectId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.IsAdmin == true && u.SubjectId == subjectId);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
