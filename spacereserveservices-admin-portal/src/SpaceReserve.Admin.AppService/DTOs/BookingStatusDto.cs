using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceReserve.Admin.AppService.DTOs
{
    public class BookingStatusDto
    {
        public int Id { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
    }
}