using System.ComponentModel.DataAnnotations;

namespace SpaceReserve.Infrastructure.Entities;

public class BookingStatusModel
{
    [Key]
    public byte BookingStatusId { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
    public DateTime? DeletedDate { get; set; }
}
