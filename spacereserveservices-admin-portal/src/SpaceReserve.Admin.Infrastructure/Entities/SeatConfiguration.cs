using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class SeatConfiguration
{
    public int SeatConfigurationId { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    public short SeatId { get; set; }
    [ForeignKey("SeatId")]
    public virtual Seat? Seat { get; set; }
     public int CreatedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }
     
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int? ModifiedBy { get; set; }
    [ForeignKey("ModifiedBy")]
    public virtual User? Modifier { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? DeletedBy { get; set; }
    [ForeignKey("DeletedBy")]
    public virtual User? Deleter { get; set; }
    public DateTime? DeletedDate { get; set; }

}
