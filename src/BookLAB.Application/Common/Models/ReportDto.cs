namespace BookLAB.Application.Common.Models
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public Guid? ScheduleId { get; set; }
        public Guid UserId { get; set; }

        public string? ReportType { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsResolved { get; set; }

        public int? LabRoomId { get; set; }
        public string? RoomName { get; set; }
        public string? BuildingName { get; set; }
        public string? Reason { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

}
