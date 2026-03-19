using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.Attendances.Queries.GetAttendanceList
{
    public class AttendanceStudentDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public AttendanceStatus Status { get; set; } // Present, Absent, etc.
        public DateTimeOffset? CheckInTime { get; set; }
    }
}
