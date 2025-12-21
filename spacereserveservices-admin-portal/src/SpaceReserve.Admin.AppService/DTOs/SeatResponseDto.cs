namespace SpaceReserve.Admin.AppService.DTOs;

public class SeatResponseDto
{
    public short SeatId { get; set; }
    public byte SeatNumber { get; set; }
    public char ColumnName { get; set; }
    public byte ColumnId {get; set;}
}
