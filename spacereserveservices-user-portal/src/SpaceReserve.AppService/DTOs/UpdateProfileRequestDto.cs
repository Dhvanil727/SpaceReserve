namespace SpaceReserve.AppService.DTOs;

public class UpdateProfileRequestDto
{
    public string ProfilePictureFileString { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public byte Designation { get; set; }
    public byte City { get; set; }
    public byte? Floor { get; set; }
    public byte? Column { get; set; }
    public short? Seat { get; set; }
    public byte ModeOfWork { get; set; }
    public List<byte> WorkingDays { get; set; } = new List<byte>();
}
