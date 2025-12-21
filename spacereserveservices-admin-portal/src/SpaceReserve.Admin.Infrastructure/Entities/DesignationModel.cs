using System.ComponentModel.DataAnnotations;

namespace SpaceReserve.Infrastructure.Entities;

public class DesignationModel
{
    [Key]
    public byte DesignationId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
    public DateTime? DeletedDate { get; set; }
}
