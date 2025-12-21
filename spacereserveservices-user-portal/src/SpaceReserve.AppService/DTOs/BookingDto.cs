namespace SpaceReserve.AppService.DTOs;

public class BookingDto
{
    public string Reason { get; set; } 
    public DateOnly RequestDateTime { get; set; }
    public short SeatId { get; set; }
    
}
