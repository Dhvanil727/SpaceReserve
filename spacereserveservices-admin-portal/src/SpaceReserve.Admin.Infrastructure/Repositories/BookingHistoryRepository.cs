using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Admin.Utility.Resources;
using System.Security.AccessControl;
using SpaceReserve.Admin.Infrastructure.Extensions;

namespace SpaceReserve.Admin.Infrastructure.Repositories
{
    public class BookingHistoryRepository : IBookingHistoryRepository
    {
        private readonly AppDbContext _context;

        public BookingHistoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Booking>> GetAllBookingHistoryAsync(int pageNo, int pageSize)
        {
            var bookingHistory = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.DeletedBy == null)
                .Include(b => b.Seat!.ColumnModel!.FloorModel)
                .Include(b => b.BookingStatusModel)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .ThenByDescending(b=>b.CreatedDate)
                .GetPaginated(pageNo, pageSize)
                .ToListAsync();

            return bookingHistory;
        }
        public async Task<List<Booking>> GetUpcomingBookingHistoryAsync(int pageNo, int pageSize)
        {
            var bookingHistory = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.BookingDate >= DateOnly.FromDateTime(DateTime.Now) && b.BookingDate <= DateOnly.FromDateTime(DateTime.Now).AddMonths(3) && b.DeletedBy == null)
                .Include(b => b.Seat!.ColumnModel!.FloorModel)
                .Include(b => b.BookingStatusModel)
                .Include(b => b.User)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b=>b.CreatedDate)
                .GetPaginated(pageNo, pageSize)
                .ToListAsync();

            return bookingHistory;
        }
         public async Task<List<Booking>> GetPastBookingHistoryAsync( int pageNo, int pageSize)
        {
            var bookingHistory =await _context.Bookings
                .AsNoTracking()
                .Where(b => b.BookingDate < DateOnly.FromDateTime(DateTime.Now) && b.BookingStatusId != (int)CommonResources.BookingStatus.Pending && b.DeletedDate == null)
                .Include(b => b.Seat!.ColumnModel!.FloorModel)
                .Include(b => b.BookingStatusModel)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .ThenByDescending(b=>b.CreatedDate)
                .GetPaginated(pageNo,pageSize)
                .ToListAsync();
                
            return bookingHistory; 
        }
    }
}