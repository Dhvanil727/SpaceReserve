
namespace SpaceReserve.AppService.DTOs;

public class UserDto
{
    public string ProfilePictureFileString { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DesignationModelDto Designation { get; set; } = new DesignationModelDto();
    public ModeOfWorkDto ModeOfWork { get; set; } = new ModeOfWorkDto();
    public CityModelDto? City { get; set; } = new CityModelDto();
    public FloorDto? Floor { get; set; } = new FloorDto();
    public ColumnDto? Column { get; set; } = new ColumnDto();
    public SeatDto? Seat { get; set; } = new SeatDto();
    public List<WorkingDayDto> Days { get; set; } = new List<WorkingDayDto>();
    public bool Active { get; set; } 
    
}
