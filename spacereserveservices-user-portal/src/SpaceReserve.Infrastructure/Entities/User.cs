using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }
    public string SubjectId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CreatedBy { get; set; } = 1;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImageName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; } = true;
    public byte? ModeOfWorkId { get; set; }
    [ForeignKey("ModeOfWorkId")]
    public virtual ModeOfWorkModel? ModeOfWorkModel { get; set; }

    public byte? DesignationId { get; set; }
    [ForeignKey("DesignationId")]
    public virtual DesignationModel? DesignationModel { get; set; }
    public byte? CityId { get; set; }
    [ForeignKey("CityId")]
    public virtual CityModel? CityModel { get; set; }
    public bool IsAdmin { get; set; } = false;

    public virtual ICollection<UserWorkingDay> UserWorkingDays { get; set; } = new List<UserWorkingDay>();
    public virtual ICollection<Booking> Bookings { get; set;} = new List<Booking>();

}