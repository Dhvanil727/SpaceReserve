using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class UserWorkingDay
{
    public int UserWorkingDayId { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User? User { get; set; } 
    public byte WorkingDayId { get; set; }
    [ForeignKey("WorkingDayId")]
    public virtual WorkingDay? WorkingDay { get; set; }
    public int CreatedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }
    public DateTime CreatedDate { get; set; } 
    public int? ModifiedBy { get; set; }
    [ForeignKey("ModifiedBy")]
    public virtual User? Modifier { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? DeletedBy { get; set; }
    [ForeignKey("DeletedBy")]
    public virtual User? Deleter { get; set; }
    public DateTime? DeletedDate { get; set; }

}
