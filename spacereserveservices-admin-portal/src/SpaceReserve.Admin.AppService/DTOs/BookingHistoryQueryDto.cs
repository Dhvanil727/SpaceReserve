using SpaceReserve.Admin.AppService.Enums;

namespace SpaceReserve.Admin.AppService.DTOs;

public class BookingHistoryQueryDto
{
    public string? Search { get; set; } = null;
    public int Sort { get; set; }=Convert.ToInt32(BookingFilter.All);
    public int PageNo { get; set; } 
    public int PageSize { get; set; } 
}
