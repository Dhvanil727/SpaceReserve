using System.ComponentModel.DataAnnotations;

namespace SpaceReserve.Infrastructure.Entities;

public class CityModel
{
    [Key]
    public byte CityId { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
    public DateTime? DeletedDate { get; set; }
    public virtual ICollection<FloorModel> FloorModels { get; set; } = new List<FloorModel>();
}
