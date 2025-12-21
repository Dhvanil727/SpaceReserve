using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class ColumnModel
{
    [Key]
    public byte ColumnId { get; set; }
    public byte FloorId { get; set; }
    [ForeignKey("FloorId")]
    public virtual FloorModel? FloorModel { get; set; }
    public char Column { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
