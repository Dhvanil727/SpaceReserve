using Microsoft.EntityFrameworkCore;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Enum;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.Infrastructure.Repositories;

public class SeatBookingRepository : ISeatBookingRepository

{
    private readonly AppDbContext _context;

    public SeatBookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(User? Owner, Booking? Bookings, int numberOfBookings)> GetSeatBookingDetailsAsync(short seatId, DateOnly date)
    {
        var seatOwner = await _context.SeatConfigurations
            .AsNoTracking()
            .Where(sc => sc.SeatId == seatId && sc.DeletedDate == null)
            .Include(sc => sc.User)
                .ThenInclude(u => u.DesignationModel)
            .Select(sc => sc.User)
            .FirstOrDefaultAsync();

        var bookings = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.SeatId == seatId && b.BookingDate == date && b.DeletedDate == null && b.BookingStatusId==(byte)BookingStatus.Accepted)
            .Include(b => b.User)
                .ThenInclude(u => u.DesignationModel)
            .FirstOrDefaultAsync();

        var numberOfBookings = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.SeatId == seatId && b.BookingDate == date && b.DeletedDate == null && b.BookingStatusId== (byte)BookingStatus.Pending)
            .CountAsync();

        return (seatOwner, bookings, numberOfBookings);
    }

    public async Task<Booking> BookSeat(Booking booking, string type)
    {
        booking.Type = type;
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }


    public async Task<List<Seat>> GetAllSeatsAsync(DateOnly date, byte cityId, byte floorId)
    {
        return await _context.Seats
              .AsNoTracking()
              .AsSplitQuery()
              .Include(s => s.ColumnModel!)
                  .ThenInclude(c => c.FloorModel!)
                      .ThenInclude(f => f.CityModel)
              .Include(s => s.SeatConfigurations!.Where(sc => sc.DeletedDate == null))
                  .ThenInclude(sc => sc.User!)
                      .ThenInclude(u => u.ModeOfWorkModel!)
              .Include(s => s.SeatConfigurations!.Where(sc => sc.DeletedDate == null))
                  .ThenInclude(sc => sc.User!)
                      .ThenInclude(u => u.DesignationModel!)
              .Include(s => s.SeatConfigurations!.Where(sc => sc.DeletedDate == null))
                  .ThenInclude(sc => sc.User!)
                      .ThenInclude(u => u.UserWorkingDays!.Where(uw => uw.DeletedDate == null))
              .Include(s => s.Bookings!.Where(b => b.BookingDate == date && b.DeletedDate == null))
                  .ThenInclude(b => b.User!)

          .Where(s =>
              s.ColumnModel!.FloorModel!.CityModel!.CityId == cityId &&
              s.ColumnModel.FloorModel.FloorId == floorId &&
              s.ColumnModel.FloorModel.CityModel.DeletedDate == null &&
              s.ColumnModel.FloorModel.DeletedDate == null)
          .ToListAsync();
    }

    public async Task<bool> SeatExistsAsync(short seatId)
    {
        return await _context.Seats.AnyAsync(s => s.SeatId == seatId);
    }

    public async Task<(SeatConfiguration? seatConfiguration, User? user, string? city, string? floor, string? seatNumber)> GetSeatDetailBySeatIdAsync(short seatId)
    {

        var seatConfiguration = await _context.SeatConfigurations
            .Where(sc => sc.SeatId == seatId && sc.DeletedDate == null)
            .Include(sc => sc.Seat!.ColumnModel!.FloorModel!.CityModel)
            .Include(sc => sc.User)
            .FirstOrDefaultAsync();

        var cityName = seatConfiguration?.Seat?.ColumnModel?.FloorModel?.CityModel?.City;
        var floorName = seatConfiguration?.Seat?.ColumnModel?.FloorModel?.Floor;
        var seatNumber = seatConfiguration?.Seat!.ColumnModel!.Column + "" + seatConfiguration?.Seat!.SeatNumber;
        return (seatConfiguration, seatConfiguration?.User, cityName, floorName, seatNumber);
    }
    public async Task<(string? city, string? floor, string? seatNumber)> GetUnassignSeatDetailBySeatIdAsync(short seatId)
    {
        var seat = await _context.Seats
           .Where(s => s.SeatId == seatId)
           .Include(s => s.ColumnModel!.FloorModel!.CityModel)
           .FirstOrDefaultAsync();

        var city = seat.ColumnModel?.FloorModel?.CityModel?.City;
        var floor = seat.ColumnModel?.FloorModel?.Floor;
        var seatNum = seat.ColumnModel?.Column + "" + seat.SeatNumber;
        return (city, floor, seatNum);

    }

    public async Task<int> GetSeatRequestCountAsync(short seatId, DateOnly date)
    {
        return await _context.Bookings
        .Where(b => b.SeatId == seatId && b.BookingDate == date && b.DeletedDate == null && b.BookingStatusId != (byte)BookingStatus.Cancelled && b.BookingStatusId != (byte)BookingStatus.Rejected)
        .CountAsync();
    }

    public async Task<User?> GetAdminDetails()
    {
        return await _context.Users
            .Where(u => u.IsAdmin && u.DeletedDate == null && u.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<(int limit, int seatCount)> CheckLimitOfUserAsync(string userId, DateOnly date, short seatId)
    {
        var limit = await _context.Bookings
            .Include(b => b.User)
            .Where(b => b.User!.SubjectId == userId && b.BookingDate == date && b.DeletedDate == null && b.BookingStatusId != (byte)BookingStatus.Cancelled  && b.BookingStatusId != (byte)BookingStatus.Rejected)
            .CountAsync();

        var seatRequestCount = await _context.Bookings
            .Include(b => b.User)
            .Where(b => b.User!.SubjectId == userId && b.BookingDate == date && b.DeletedDate == null && b.SeatId == seatId  && b.BookingStatusId != (byte)BookingStatus.Cancelled && b.BookingStatusId != (byte)BookingStatus.Rejected)
            .CountAsync();

        return (limit, seatRequestCount);
    }
    public async Task<string?> ModeOfUser(string userSubjectId)
    {
        var modeOfUser = await _context.Users
            .Include(u => u.ModeOfWorkModel)
            .Where(u => u.SubjectId == userSubjectId && u.DeletedDate == null)
            .Select(u => u.ModeOfWorkModel!.ModeOfWork)
            .FirstOrDefaultAsync();
        return modeOfUser;
    }
    public async Task<List<byte>> WorkingDaysOfHybridUser(string userSubjectId)
    {
        var userId = await _context.Users
            .Where(u => u.SubjectId == userSubjectId && u.ModeOfWorkModel!.ModeOfWork == SeatConfigurationResources.ModeHybrid && u.DeletedDate == null)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        var workingDays = await _context.UserWorkingDays
            .Where(u => u.UserId == userId && u.DeletedDate == null)
            .Select(u => u.WorkingDayId)
            .ToListAsync();

        return workingDays;
    }

    public async Task<bool> CheckUserBookingStatus(string userSubId, DateOnly date)
    {
        var booking = await _context.Bookings
            .Include(b => b.User)
            .Where(b => b.User!.SubjectId == userSubId && b.BookingDate == date && b.DeletedDate == null)
            .Select(b => b.BookingStatusId).FirstOrDefaultAsync();
        return booking == (byte)BookingStatus.Accepted;
    }

    public async Task<(Seat? seat, byte? designationId, int? bookingStatus, SeatConfiguration? seatConfiguration,byte? mode)> SeatAvailable(int seatId, DateOnly date)
    {
        var seat = await _context.Seats.Where(s => s.SeatId == seatId).FirstOrDefaultAsync();
        var userId = await _context.SeatConfigurations.Where(s => s.SeatId == seatId && s.DeletedDate == null).Select(s => s.UserId).FirstOrDefaultAsync();
        var designationId = await _context.SeatConfigurations
            .Include(s => s.User)
            .Where(s => s.SeatId == seatId && s.UserId == userId && s.DeletedDate == null)
            .Select(s => s.User!.DesignationId).FirstOrDefaultAsync();
        var bookingStatus = await _context.Bookings
            .Where(b => b.SeatId == seatId && b.BookingDate == date && b.DeletedDate == null)
            .Select(b => b.BookingStatusId).FirstOrDefaultAsync();
        var hybridUserSeat = await _context.SeatConfigurations
            .Include(sc => sc.User)
                .ThenInclude(u => u!.UserWorkingDays)
            .Where(sc => sc.SeatId == seatId && sc.UserId == userId)
            .FirstOrDefaultAsync();
            
        var modeOfUser = await _context.SeatConfigurations
            .Include(sc => sc.User)
            .Where(sc => sc.SeatId == seatId && sc.UserId == userId && sc.DeletedDate == null)
            .Select(sc => sc.User!.ModeOfWorkId)
            .FirstOrDefaultAsync();
        return (seat, designationId, bookingStatus, hybridUserSeat, modeOfUser);
    }
}