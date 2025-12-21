
using AutoMapper;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Utility.Enum;
using SpaceReserve.Utility.Resources;

namespace SpaceReserve.AppService.Services;

public class ReferenceService : IReferenceService
{
    private readonly IReferenceRepository _referenceRepository;
    private readonly IMapper _mapper;
    public ReferenceService(IReferenceRepository referenceRepository, IMapper mapper)
    {
        _referenceRepository = referenceRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DesignationModelDto>> GetDesignationsAsync()
    {
        var designations = await _referenceRepository.GetDesignationsAsync();
        return _mapper.Map<IEnumerable<DesignationModelDto>>(designations);
    }

    public async Task<IEnumerable<WorkingDayDto>> GetWorkingDaysAsync()
    {
        var data = await _referenceRepository.GetAllWorkingDaysAsync();
        return _mapper.Map<List<WorkingDayDto>>(data);
    }

    public async Task<IEnumerable<SeatDto>> GetSeatsByColumnIdAsync(byte columnId)
    {
         if (columnId < (byte)Column.Zero || columnId > (byte)Column.Eighty)
        {
            throw new ArgumentException(CommonResource.CheckColumnId);
        }
        var entities = await _referenceRepository.GetSeatsByColumnIdAsync(columnId);
        return _mapper.Map<IEnumerable<SeatDto>>(entities);
    }

    public async Task<List<short>> GetSeatsConfigurationByColumnIdAsync(string subjectId, byte columnId)
    {
        var seats = await _referenceRepository.GetSeatsConfigurationByColumnIdAsync(subjectId, columnId);
        return seats;
    }

    public async Task<List<short>> GetUnderMaintainanceSeat()
    {
        return await _referenceRepository.GetUnderMaintainanceSeat();
    }

    public async Task<IEnumerable<ModeOfWorkDto>> GetAllModeOfWorksAsync()
    {
        var modes = await _referenceRepository.GetAllModeOfWorksAsync();
        return _mapper.Map<IEnumerable<ModeOfWorkDto>>(modes);
    }

    public async Task<List<ColumnDto>> GetColumnsByFloorIdAsync(int floorId)
    {
        var columns = await _referenceRepository.GetColumnsByFloorIdAsync(floorId);
        var columnDtos = _mapper.Map<List<ColumnDto>>(columns);

        return columnDtos ?? new List<ColumnDto>();
    }

    public async Task<IEnumerable<CityModelDto>> GetAllCityAsync()
    {
        var cities = await _referenceRepository.GetAllCityAsync();
        return _mapper.Map<IEnumerable<CityModelDto>>(cities);
    }

    public async Task<List<FloorDto>> GetFloorByCityId(byte cityId)
    {
        if (cityId < 1 || cityId > 2)
        {
            throw new ArgumentException(CommonResource.CheckCityId);
        }
        var floor = await _referenceRepository.GetFloorByCityIdAsync(cityId);
        return _mapper.Map<List<FloorDto>>(floor);
    }

    public async Task<List<short>> GetSeatsIdFromBookingAsync()
    {
        return await _referenceRepository.GetSeatsIdFromBookingAsync();
    }
}
