using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.DTOs
{
    public class GetBookingStatsRequestDTO
    {
        public string startDate { get; set; } = "1000-01-01";
        public string endDate { get; set; } = "9999-12-31";
    }
}
