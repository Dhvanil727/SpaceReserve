namespace SpaceReserve.AppService.DTOs;

public class AddBookingDto
{
    
    public string Reason { get; set; } 
    public string RequestDateTime { get; set; } = string.Empty;
    public short SeatId { get; set; }
}
