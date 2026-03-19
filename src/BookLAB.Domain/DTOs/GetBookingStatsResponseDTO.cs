using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.DTOs
{
    public class GetBookingStatsResponseDTO
    {
        public int totalAccepted { get; set; }
        public int totalPending { get; set; }
        public int totalRejected { get; set; }
        public int upcomingBookings { get; set; }
    }
}
