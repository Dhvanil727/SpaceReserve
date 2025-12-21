namespace SpaceReserve.Admin.AppService.DTOs;

public class RegisteredUserDto
{
    public int EmployeeId { get; set; }
    public string SubjectId { get; set; } = string.Empty;
    public string fullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public bool Status { get; set; } 
}
