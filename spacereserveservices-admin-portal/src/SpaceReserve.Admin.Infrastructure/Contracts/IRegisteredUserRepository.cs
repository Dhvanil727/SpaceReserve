using Microsoft.EntityFrameworkCore.Storage;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Contracts;

public interface IRegisteredUserRepository
{
    Task<List<Booking>> GetUpcomingBookingHistoryOfUserAsync(int pageNo, int pageSize, int userId);
    Task<List<Booking>> GetAllBookingHistoryOfUserAsync(int pageNo, int pageSize, int userId);
    Task<List<Booking>> GetPastBookingHistoryOfUserAsync(int pageNo, int pageSize, int userId);

    Task<User?> GetUserProfileBySubjectIdAsync(int userId);
    Task<SeatConfiguration?> GetSeatConfigurationByUserIdAsync(int userId);
    Task<Seat?> GetSeatByIdAsync(short seatId);
    Task<ColumnModel?> GetColumnByIdAsync(short columnId);
    Task<FloorModel?> GetFloorByIdAsync(short floorId);
    Task<List<short>> GetAllAssignedSeatIdsAsync();

    Task<IEnumerable<User>> GetAllUsersAsync(int pageNo, int pageSize);

    Task<User?> GetUserByIdAsync(int userId);
    Task UpdateUserAsync(User user);
    Task<List<User>> GetUserByUserId(int userId);
    Task<SeatConfiguration> GetSeatIdOfUserAsync(int userId);
    Task<List<Booking>> GetAllBookingsOfSeatIdAsync(int seatId);
    Task<List<Booking>> GetBookingsByUserId(int userId);
    Task<List<SeatConfiguration>> GetSeatConfigurationsByUserId(int userId);
    Task<List<UserWorkingDay>> GetUserWorkingDaysByUserId(int userId);
    Task<IDbContextTransaction> BeginTransactionAndRollbackAsync();
    Task<User?> GetAdminBySubjectIdAsync(string subjectId);
    Task SaveChangesAsync();
}