namespace SpaceReserve.AppService.DTOs;

public class SeatDto
{
    public short Id { get; set; }
    public byte SeatNumber { get; set; }
    public bool Booked { get; set; } = false;

}
