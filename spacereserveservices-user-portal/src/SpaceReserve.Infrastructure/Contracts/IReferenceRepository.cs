using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Contracts;

public interface IReferenceRepository
{
    Task<IEnumerable<DesignationModel>> GetDesignationsAsync();
    Task<IEnumerable<WorkingDay>> GetAllWorkingDaysAsync();
    Task<IEnumerable<Seat>> GetSeatsByColumnIdAsync(byte columnId);
    Task<List<short>> GetSeatsConfigurationByColumnIdAsync(string subjectId, byte columnId);
    Task<IEnumerable<ModeOfWorkModel>> GetAllModeOfWorksAsync();
    Task<List<ColumnModel>>GetColumnsByFloorIdAsync(int FloorId);
    Task<IEnumerable<CityModel>> GetAllCityAsync();
    Task<List<FloorModel>> GetFloorByCityIdAsync(byte cityId);
    Task<List<short>> GetSeatsIdFromBookingAsync();
    Task<List<short>> GetUnderMaintainanceSeat();
}
