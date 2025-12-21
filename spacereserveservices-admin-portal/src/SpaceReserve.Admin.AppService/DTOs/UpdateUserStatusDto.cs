namespace SpaceReserve.Admin.AppService.DTOs;

public class UpdateUserStatusDto
{
    public int UserId { get; set; }
    public string SubjectId { get; set; } 
    public bool IsActive { get; set; }
}
