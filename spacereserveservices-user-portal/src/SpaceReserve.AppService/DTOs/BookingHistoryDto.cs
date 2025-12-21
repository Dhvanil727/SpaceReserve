namespace SpaceReserve.AppService.DTOs;

public class BookingHistoryDto
{
    public int RequestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BookingDate { get; set; } = string.Empty;
    public string RequestedDate { get; set; }=string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Floor { get; set; }=string.Empty;
    public string DeskNumber { get; set; } = string.Empty;
    public byte RequestStatus { get; set; }
}

    