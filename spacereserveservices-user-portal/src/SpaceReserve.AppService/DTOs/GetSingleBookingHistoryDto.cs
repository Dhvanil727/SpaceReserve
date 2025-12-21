using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceReserve.AppService.DTOs.RequestHistory
{
    public class GetSingleBookingtHistoryDto
    {
        public int BookingId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime? RequestDate { get; set; }
        public DateOnly? BookingDate { get; set; }
        public int? FloorNo { get; set; }    
        public string? DeskNumber { get; set; } = string.Empty;
        public int? Status { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}