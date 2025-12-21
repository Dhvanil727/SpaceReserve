namespace SpaceReserve.AppService.DTOs;

public class SeatDetailsDto
{
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CountOfRequest { get; set; }
    public string Message {get;set;}
    public TemporarySeatOwnerDto? TemporarySeatOwnerDto { get; set; }
}
