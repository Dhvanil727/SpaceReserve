using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceReserve.Admin.AppService.DTOs
{
    public class GetSingleRequestHistoryDto
    {
        public int RequestId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? RequestDate { get; set; } = string.Empty;
        public string? RequestedFor { get; set; } = string.Empty;
        public string? FloorNo { get; set; } = string.Empty;  
        public string? DeskNo { get; set; } = string.Empty;
        public int? Status { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}