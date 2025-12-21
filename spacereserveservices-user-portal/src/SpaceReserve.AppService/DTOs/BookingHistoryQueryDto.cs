namespace SpaceReserve.AppService.DTOs;

public class BookingHistoryQueryDto
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public string Search { get; set; } = string.Empty;
    public int Sort { get; set; }
}
