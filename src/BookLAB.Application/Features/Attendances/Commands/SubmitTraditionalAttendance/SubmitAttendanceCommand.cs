using MediatR;

namespace BookLAB.Application.Features.Attendances.Commands.SubmitTraditionalAttendance
{
    public class SubmitAttendanceCommand : IRequest<bool>
    {
        public Guid ScheduleId { get; set; }
        public List<AttendanceItemDto> AttendanceItems { get; set; } = new();
    }
}