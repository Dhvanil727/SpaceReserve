using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceReserve.Infrastructure.Entities;

public class FloorModel 
{
    [Key]
    public byte FloorId { get; set; }
    public byte CityId { get; set; }
    [ForeignKey("CityId")]
    public virtual CityModel? CityModel { get ; set ; }
    public string Floor { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
    public DateTime? DeletedDate { get; set; }
    public virtual ICollection<ColumnModel> ColumnModels { get; set; } = new List<ColumnModel>();
}
