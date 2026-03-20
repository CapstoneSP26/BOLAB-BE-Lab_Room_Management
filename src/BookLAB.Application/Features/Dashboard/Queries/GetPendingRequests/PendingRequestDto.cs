namespace BookLAB.Application.Features.Dashboard.Queries.GetPendingRequests
{
    public class PendingRequestDto
    {
        public Guid BookingId { get; set; }
        public string LabRoomName { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public string RequesterName { get; set; } = string.Empty;
        public string RequesterEmail { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ExpectedStudents { get; set; }
        public string? Purpose { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
