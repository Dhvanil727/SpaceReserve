using Microsoft.EntityFrameworkCore;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;
using static SpaceReserve.Admin.Utility.Resources.CommonResources;
namespace SpaceReserve.Admin.Infrastructure.Repositories
{
    public class SeatConfigurationRepository : ISeatConfigurationRepository
    {
        private readonly AppDbContext _context;
        public SeatConfigurationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddSeatAsync(List<Seat> seat)
        {
            _context.Seats.AddRange(seat);
            await _context.SaveChangesAsync();

        }
        public async Task<List<(byte columnId,int seatCount,List<byte> seatNumbers)>> GetSeatCountAsync( List<byte> columnId)
        {
            return await _context.ColumnModels
            .Include(c => c.Seats)
                .Where(s => columnId.Contains(s.ColumnId) && s.DeletedDate == null)
                .Select(b => new ValueTuple<byte, int , List<byte>>(b.ColumnId, b.Seats.Count, b.Seats.Select(s => s.SeatNumber).ToList()))
                .ToListAsync();
               
        }
        public async Task<User?> GetUserByIdAsync(string subjectId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.SubjectId == subjectId);
        }
        public async Task<List<Seat>> GetSeatsByFloorIdAsync(byte cityId, byte floorId)
        {
            return await _context.Seats
              .AsNoTracking()
              .AsSplitQuery()
              .Include(s => s.ColumnModel!)
                  .ThenInclude(c => c.FloorModel!)
              .Include(s => s.SeatConfigurations!.Where(s => s.DeletedDate == null))
                  .ThenInclude(sc => sc.User!)
                      .ThenInclude(u => u.ModeOfWorkModel!)
              .Include(s => s.SeatConfigurations!.Where(s => s.DeletedDate == null))
                  .ThenInclude(sc => sc.User!)
                      .ThenInclude(u => u.DesignationModel!)
              .Include(s => s.SeatConfigurations!.Where(s => s.DeletedDate == null))
                  .ThenInclude(sc => sc.User!)
                      .ThenInclude(u => u.UserWorkingDays!.Where(u => u.DeletedDate == null))
              .Include(s => s.Bookings!.Where(b => b.BookingDate == DateOnly.FromDateTime(DateTime.Now) && b.DeletedDate == null))
                  .ThenInclude(b => b.User!)
              .Where(
                    s => s.ColumnModel!.FloorModel!.FloorId == floorId &&
                    s.ColumnModel!.FloorModel!.CityModel!.CityId == cityId
                )
              .ToListAsync();
        }
        public async Task<Seat?> GetSeatByIdAsync(short seatId)
        {
            return await _context.Seats
                .Include(s => s.SeatConfigurations)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.SeatId == seatId);
        }
        public async Task<List<Booking>> GetBookingsForSeatOnDateAsync(short seatId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Seat)
                    .ThenInclude(b => b!.ColumnModel)
                        .ThenInclude(b => b!.FloorModel)
                            .ThenInclude(b => b!.CityModel)
                .Where(b => b.SeatId == seatId && b.BookingStatusId != (byte)BookingStatus.Cancelled && b.BookingStatusId != (byte)BookingStatus.Rejected && b.DeletedDate == null)
                .ToListAsync();
        }
        public async Task UpdateSeatAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveSeatConfigurationAsync(string subjectId, SeatConfiguration config)
        {
            config.DeletedBy = await GetAdminIdAsync(subjectId);
            config.DeletedDate = DateTime.Now;
            _context.SeatConfigurations.Update(config);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetAdminIdAsync(string subjectId)
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.IsAdmin == true && u.SubjectId == subjectId);
            return admin!.UserId;
        }
        public async Task<SeatConfiguration?> GetSeatConfigurationBySeatIdAsync(short seatId)
        {
            var seatConfiguration = await _context.SeatConfigurations
                .Include(sc => sc.User)
                .Include(sc => sc.Seat)
                    .ThenInclude(sc => sc!.ColumnModel)
                        .ThenInclude(sc => sc!.FloorModel)
                            .ThenInclude(sc => sc!.CityModel)
                .FirstOrDefaultAsync(sc => sc.SeatId == seatId && sc.DeletedDate == null);
            return seatConfiguration;
        }
    }
}



