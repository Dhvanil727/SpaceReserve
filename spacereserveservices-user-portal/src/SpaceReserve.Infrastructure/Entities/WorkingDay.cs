namespace SpaceReserve.Infrastructure.Entities;

public class WorkingDay
{
    public byte WorkingDayId { get; set; }
    public string WorkDay { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
    public DateTime? DeletedDate { get; set; }
     public virtual ICollection<UserWorkingDay>? UserWorkingDays { get; set; } 
}
