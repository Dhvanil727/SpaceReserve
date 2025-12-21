using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.AppService.Contracts;

public interface ISeatConfigurationService
{
    Task AddSeatAsync(List<AddSeatRequestDto> addSeatRequestDto, int userId);
    Task<GetSeatConfigurationResponseDto> GetSeatsByFloorIdAsync(byte cityId, byte floorId);
    Task<List<(byte columnId,int seatCount,List<byte> seatNumbers)>> GetSeatCountAsync( List<byte> columnId);
    Task<User?> GetUserByIdAsync(string subjectId);
    Task<string> UpdateSeatStatusAsync(string subjectId, SeatStatusUpdateDto seatStatusUpdateDto);
    
}
