using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    public byte BookingStatusId { get; set; }
    [ForeignKey("BookingStatusId")]
    public virtual BookingStatusModel? BookingStatusModel { get; set; }
    public short SeatId { get; set; }
    [ForeignKey("SeatId")]
    public virtual Seat? Seat { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateOnly BookingDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    [ForeignKey("Creator")]
    public int CreatedBy { get; set; }
    public virtual User? Creator { get; set; }
 
    public int?  ModifiedBy { get; set; }
    [ForeignKey("ModifiedBy")]
    public virtual User? Modifier { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? DeletedBy { get; set; }
    [ForeignKey("DeletedBy")]
    public virtual User? Deleter { get; set; }
    public DateTime? DeletedDate { get; set; }
    
}
