using SpaceReserve.AppService.DTOs;

namespace SpaceReserve.AppService.Contracts;

public interface IReferenceService
{
    Task<IEnumerable<DesignationModelDto>> GetDesignationsAsync();
    Task<IEnumerable<WorkingDayDto>> GetWorkingDaysAsync();
    Task<IEnumerable<SeatDto>> GetSeatsByColumnIdAsync(byte columnId);
    Task<List<short>> GetSeatsConfigurationByColumnIdAsync(string subjectId, byte columnId);
    Task<IEnumerable<ModeOfWorkDto>> GetAllModeOfWorksAsync();
    Task<List<ColumnDto>> GetColumnsByFloorIdAsync(int floorId);
    Task<IEnumerable<CityModelDto>> GetAllCityAsync();
    Task<List<FloorDto>> GetFloorByCityId(byte cityId);
    Task<List<short>> GetSeatsIdFromBookingAsync();

    Task<List<short>> GetUnderMaintainanceSeat();
}
