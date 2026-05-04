namespace BookLAB.Domain.DTOs
{
    public class MonthlyDataPointDTO
    {
        public int month { get; set; }
        public string label { get; set; } = string.Empty;
        public int value { get; set; }
    }

    public class DashboardStatisticsSeriesDTO
    {
        public List<MonthlyDataPointDTO> monthlyIncidents { get; set; } = new();
        public List<MonthlyDataPointDTO> monthlyApprovedBookings { get; set; } = new();
        public List<MonthlyDataPointDTO> monthlyPendingBookings { get; set; } = new();
    }

    public class DashboardStatsResponseDTO
    {
        public int pendingBookings { get; set; }
        public int approvedBookings { get; set; }
        public int unresolvedIncidents { get; set; }
        public int availableRooms { get; set; }
        public int totalRooms { get; set; }

        public int year { get; set; }
        public List<MonthlyDataPointDTO> monthlyBookings { get; set; } = new();
        public DashboardStatisticsSeriesDTO statistics { get; set; } = new();
    }
}
