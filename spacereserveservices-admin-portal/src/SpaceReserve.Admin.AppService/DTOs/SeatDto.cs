namespace SpaceReserve.Admin.AppService.DTOs;

public class SeatDto
{
    public short SeatId { get; set; }
    public byte SeatNumber { get; set; }
     public bool Booked { get; set; } = false;
}
