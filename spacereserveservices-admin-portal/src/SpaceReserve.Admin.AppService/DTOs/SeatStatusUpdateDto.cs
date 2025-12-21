using System.ComponentModel.DataAnnotations;

namespace SpaceReserve.Admin.AppService.DTOs;

public class SeatStatusUpdateDto
{
    public bool unAssigned { get; set; } 
    public bool isAvailable { get; set; }
    public short seatId { get; set; } 
}
