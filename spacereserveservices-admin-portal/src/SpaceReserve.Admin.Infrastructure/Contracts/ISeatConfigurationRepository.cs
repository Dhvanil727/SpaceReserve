using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.Infrastructure.Contracts
{
    public interface ISeatConfigurationRepository
    {
       Task<Seat?> GetSeatByIdAsync(short seatId);
        Task<SeatConfiguration?> GetSeatConfigurationBySeatIdAsync(short seatId);
        Task<List<Booking>> GetBookingsForSeatOnDateAsync(short seatId);
        Task UpdateSeatAsync(Seat seat);
        Task UpdateBookingAsync(Booking booking);
        Task RemoveSeatConfigurationAsync(string subjectId, SeatConfiguration config);
        Task<int> GetAdminIdAsync(string subjectId); 
        Task<List<Seat>> GetSeatsByFloorIdAsync(byte cityId, byte floorId);
        Task AddSeatAsync(List<Seat> seat);
        Task<List<(byte columnId,int seatCount,List<byte> seatNumbers)>> GetSeatCountAsync( List<byte> columnId);
        Task<User?>GetUserByIdAsync(string userId);
    }
}
