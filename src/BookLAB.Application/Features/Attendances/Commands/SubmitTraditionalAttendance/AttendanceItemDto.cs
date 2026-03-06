
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.Attendances.Commands.SubmitTraditionalAttendance
{
    public class AttendanceItemDto
    {
        public Guid UserId { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
