namespace BookLAB.Domain.DTOs
{
    public class DashboardLecturerBookingRequestDto
    {
        public string lecturerId { get; set; } = string.Empty;
        public string lecturerName { get; set; } = string.Empty;
        public string lecturerEmail { get; set; } = string.Empty;
        public int bookingRequestCount { get; set; }
        public int approvedBookingCount { get; set; }
        public int roomCount { get; set; }
        public DateTimeOffset? latestRequestAt { get; set; }
    }

    public class DashboardIncidentReportsInSchedulesDto
    {
        public int totalIncidentReports { get; set; }
        public int totalSchedulesWithIncidentReports { get; set; }
    }

    public class DashboardLabUtilizationDto
    {
        public double utilizationRate { get; set; }
        public int usedRooms { get; set; }
        public int totalRooms { get; set; }
        public int usedSchedules { get; set; }
        public int totalSchedules { get; set; }
    }

    public class DashboardRoomBookingStatDto
    {
        public string roomId { get; set; } = string.Empty;
        public string roomName { get; set; } = string.Empty;
        public string buildingName { get; set; } = string.Empty;
        public int bookingCount { get; set; }
        public int approvedBookingCount { get; set; }
        public int incidentReportCount { get; set; }
        public double utilizationRate { get; set; }
    }

    public class DashboardOverviewDto
    {
        public int totalBookings { get; set; }
        public int pendingBookings { get; set; }
        public int approvedBookings { get; set; }
        public int rejectedBookings { get; set; }
        public int totalIncidents { get; set; }
        public int unresolvedIncidents { get; set; }
        public int totalRooms { get; set; }
        public int availableRooms { get; set; }
        public int totalStudents { get; set; }
        public int totalLecturers { get; set; }
        public double averageBookingDuration { get; set; }
        public string mostBookedRoom { get; set; } = string.Empty;
        public int busiestHourOfDay { get; set; }
        public List<int> monthlyBookings { get; set; } = new();
        public List<int> monthlyIncidents { get; set; } = new();
        public int approvedBookingsToday { get; set; }
        public int checkedInBookingsToday { get; set; }
        public int noCheckInBookingsToday { get; set; }
        public double checkInCompliancePercentage { get; set; }
        public int approvedBookingRequestsToday { get; set; }
        public List<DashboardLecturerBookingRequestDto> lecturerBookingRequests { get; set; } = new();
        public DashboardIncidentReportsInSchedulesDto incidentReportsInSchedules { get; set; } = new();
        public DashboardLabUtilizationDto labUtilization { get; set; } = new();
        public List<DashboardRoomBookingStatDto> roomBookingStats { get; set; } = new();
        public string scope { get; set; } = "LAB_MANAGER";
        public DateTimeOffset generatedAt { get; set; }
    }
}
