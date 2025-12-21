namespace SpaceReserve.AppService.DTOs;

public class SeatRequestDto
{
    public string Date { get; set; } = string.Empty;
    public byte CityId { get; set; }
    public byte FloorId { get; set; }
}
