namespace SpaceReserve.Admin.AppService.DTOs;

public class GetSeatConfigurationResponseDto
{
    public List<SeatResponseDto> UnavailableSeat { get; set; } = new();
    public List<SeatResponseDto> AvailableforBookingSeat { get; set; } = new();
    public List<SeatResponseDto> BookedSeat { get; set; } = new();
    public List<SeatResponseDto> ReservedSeat { get; set; } = new();
    public List<SeatResponseDto> UnallocatedSeat { get; set; } = new();
}
