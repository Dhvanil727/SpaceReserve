using SpaceReserve.AppService.DTOs;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.AppService.Contracts;

public interface ISeatBookingService
{
    Task<List<SeatViewDto>> GetAllSeatsAsync(DateOnly date, byte cityId, byte floorId);
    public Task<Result<string>> BookSeat(BookingDto bookingDto,string userSubId);
    Task<SeatDetailsDto?> GetSeatDetailsAsync(short seatId, DateOnly date);
   
}
