namespace SpaceReserve.AppService.DTOs.RequestHistory;

public class RequestHistoryDTO
{
     public string Name { get; set; } = string.Empty;
     public string RequestDate { get; set; } = string.Empty;
     public string RequestFor { get; set; } = string.Empty;
     public string EmailId  { get; set; } = string.Empty;
     public string FloorNo { get; set; } = string.Empty;
     public string DeskNo { get; set; } = string.Empty;
     public int Status { get; set; } 
     public int SeatRequestId { get; set; } 
}
