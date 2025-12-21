using SpaceReserve.Admin.AppService.Enums;

namespace SpaceReserve.Admin.AppService.DTOs;

public class BookingHistoryDto
{
    public int RequestId { get; set; }
    public string? Name{get;set;}
    public string?Email {get;set;}
    public string BookingDate { get; set; } = string.Empty;
    public string RequestedDate { get; set; }=string.Empty;
    public string? Floor { get; set; }
    public string? DeskNumber { get; set; }
    public int RequestStatus { get; set; }
}
