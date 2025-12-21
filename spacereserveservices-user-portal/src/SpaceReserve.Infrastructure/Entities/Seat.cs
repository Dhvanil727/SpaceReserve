using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class Seat
{
    public short SeatId { get; set; }
    public byte ColumnId { get; set; }
    [ForeignKey("ColumnId")]
    public virtual ColumnModel? ColumnModel { get; set; }
    public byte SeatNumber { get; set; }
    public bool IsUnderMaintenance { get; set; } = false;
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public virtual ICollection<SeatConfiguration>? SeatConfigurations { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
}
