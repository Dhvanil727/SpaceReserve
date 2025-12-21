namespace SpaceReserve.AppService.DTOs;

public class SeatViewDto
{

    public SeatDto Seat { get; set; } = new();
    public ColumnDto Column { get; set; } = new();
    public string Status { get; set; } = string.Empty;

}
