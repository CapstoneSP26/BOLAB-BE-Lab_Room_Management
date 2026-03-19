using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedules
{
    public class ScheduleDto
    {
        public Guid Id { get; init; }
        public string SubjectCode { get; init; } = string.Empty;
        public string LecturerName { get; init; } = string.Empty; // From User.FullName
        public string LabRoomName { get; init; } = string.Empty; // From LabRoom.Name
        public string SlotName { get; init; } = string.Empty;    // From SlotType.Name
        public string? GroupName { get; init; }                 // From Group.Name
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }
        public int StudentCount { get; init; }
        public string Status { get; init; } = string.Empty;      // Enum string representation
        public string Type { get; init; } = string.Empty;        // Enum string representation
    }
}
