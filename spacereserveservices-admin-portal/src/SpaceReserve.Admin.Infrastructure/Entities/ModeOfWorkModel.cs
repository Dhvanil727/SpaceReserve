using System.ComponentModel.DataAnnotations;

namespace SpaceReserve.Infrastructure.Entities;

public class ModeOfWorkModel
{
    [Key]
    public byte ModeOfWorkId { get; set; }
    public string ModeOfWork{ get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
    public DateTime? DeletedDate { get; set; }
}
