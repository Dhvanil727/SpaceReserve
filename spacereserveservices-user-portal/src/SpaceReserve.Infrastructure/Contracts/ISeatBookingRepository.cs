using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface ISeatBookingRepository
{
    Task<List<Seat>> GetAllSeatsAsync(DateOnly date, byte cityId, byte floorId);   
    Task<Booking> BookSeat(Booking booking,string type);
    Task<(User? Owner, Booking? Bookings, int numberOfBookings)> GetSeatBookingDetailsAsync(short seatId, DateOnly date);
    Task<bool> SeatExistsAsync(short seatId);
    Task<(SeatConfiguration? seatConfiguration, User? user, string? city, string? floor,string? seatNumber)> GetSeatDetailBySeatIdAsync(short seatId);
    Task<( string? city, string? floor, string? seatNumber)> GetUnassignSeatDetailBySeatIdAsync(short seatId);
    Task<int> GetSeatRequestCountAsync(short seatId, DateOnly date);
    Task<User?> GetAdminDetails();
    Task<(int limit,int seatCount)> CheckLimitOfUserAsync(string userId, DateOnly date, short seatId);
    Task<string?> ModeOfUser(string  userSubjectd);
    Task<List<byte>> WorkingDaysOfHybridUser(string userSubjectId);
    Task<bool> CheckUserBookingStatus(string userSubId,DateOnly date);
    Task<(Seat? seat,byte? designationId,int? bookingStatus,SeatConfiguration? seatConfiguration,byte? mode)> SeatAvailable(int seatId,DateOnly date);
}
