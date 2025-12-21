namespace SpaceReserve.AppService.DTOs
{
    public class CheckProfileDto
    {
        public string? ProfilePictureFileString { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Updated { get; set; }
    }
}