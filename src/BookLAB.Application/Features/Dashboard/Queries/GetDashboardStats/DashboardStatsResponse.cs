namespace BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class DashboardStatsResponse
    {
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ApprovedBookings { get; set; }
        public int RejectedBookings { get; set; }
        
        public int TotalIncidents { get; set; }
        public int UnresolvedIncidents { get; set; }
        
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        
        public int TotalStudents { get; set; }
        public int TotalLecturers { get; set; }

        // Check-in compliance (today)
        public int ApprovedBookingsToday { get; set; }
        public int CheckedInBookingsToday { get; set; }
        public int NoCheckInBookingsToday { get; set; }
        public decimal CheckInCompliancePercentage { get; set; }
        
        public decimal AverageBookingDuration { get; set; }
        public string MostBookedRoom { get; set; } = string.Empty;
        public int BusiestHourOfDay { get; set; } // 0-23
        
        // Monthly statistics (last 12 months)
        public List<int> MonthlyBookings { get; set; } = new();
        public List<int> MonthlyIncidents { get; set; } = new();
    }
}
