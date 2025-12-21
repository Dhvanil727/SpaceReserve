using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Enum;

namespace SpaceReserve.Infrastructure.Repositories;

public class ReferenceRepository : IReferenceRepository
{
    private readonly AppDbContext _context;
    public ReferenceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DesignationModel>> GetDesignationsAsync()
    {
        return await _context.DesignationModels.ToListAsync();
    }
    public async Task<IEnumerable<WorkingDay>> GetAllWorkingDaysAsync()
    {
        return await _context.WorkingDays.ToListAsync();
    }
    public async Task<IEnumerable<Seat>> GetSeatsByColumnIdAsync(byte columnId)
    {
        return await _context.Seats
            .Where(x => x.ColumnId == columnId)
            .ToListAsync();
    }

    public async Task<List<short>> GetSeatsConfigurationByColumnIdAsync(string subjectId, byte columnId)
    {
        var userId = await _context.Users
                                   .AsNoTracking()
                                   .Where(u => u.SubjectId == subjectId && u.DeletedDate == null)
                                   .Select(u => u.UserId)
                                   .FirstOrDefaultAsync();

        return await _context.SeatConfigurations
        .Include(x => x.Seat)
        .Where(x => x.Seat!.ColumnId == columnId && x.UserId != userId && x.DeletedDate == null)
        .Select(x => x.SeatId)
        .ToListAsync();
    }

    public async Task<List<short>> GetSeatsIdFromBookingAsync()
    {
        var todaysDate = DateOnly.FromDateTime(DateTime.Now);
        var seatIdsList = await _context.Bookings
            .Where(b => b.BookingDate >= todaysDate && b.DeletedDate == null && (b.BookingStatusId == (byte) BookingStatus.Pending || b.BookingStatusId == (byte) BookingStatus.Accepted))
            .Select(b => b.SeatId)
            .ToListAsync();

        return seatIdsList;
    }
    public async Task<IEnumerable<ModeOfWorkModel>> GetAllModeOfWorksAsync()
    {
        return await _context.ModeOfWorkModels
            .Where(m => m.DeletedDate == null)
            .ToListAsync();
    }
    public async Task<List<ColumnModel>> GetColumnsByFloorIdAsync(int FloorId)
    {
        return await _context.ColumnModels
            .Where(c => c.FloorId == FloorId)
            .OrderBy(c => c.Column)
            .ToListAsync();
    }
    public async Task<IEnumerable<CityModel>> GetAllCityAsync()
    {
        return await _context.CityModels.ToListAsync();
    }
    public async Task<List<FloorModel>> GetFloorByCityIdAsync(byte cityId)
    {
        return await _context.FloorModels.Where(f => f.CityId == cityId).ToListAsync();
    }

    public async Task<List<short>> GetUnderMaintainanceSeat()
    {
        return await _context.Seats
                        .AsNoTracking()
                        .Where(s => s.IsUnderMaintenance)
                        .Select(s => s.SeatId)
                        .ToListAsync();
    }
}
