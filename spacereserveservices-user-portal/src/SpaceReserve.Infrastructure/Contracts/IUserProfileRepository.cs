using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface IUserProfileRepository
{
    Task<User?> GetUserProfileBySubjectIdAsync(string subjectId);
    Task<SeatConfiguration?> GetSeatConfigurationByUserIdAsync(int userId);
    Task<Seat?> GetSeatByIdAsync(short seatId);
    Task<ColumnModel?> GetColumnByIdAsync(short columnId);
    Task<FloorModel?> GetFloorByIdAsync(short floorId);
    Task<List<short>> GetAllAssignedSeatIdsAsync();
    Task<bool> UpdateUserProfileBySubjectIdAsync(string subjectId, User user, short? seatId);
    Task<List<short>> GetAllAssignedSeats(string subjectId);
    Task<List<string?>> GetAllPhoneNumbers(string subjectId);
    Task<Seat?> FindSeatBySeatIdAndCityIdAsync(short seatId, byte cityId);
}
