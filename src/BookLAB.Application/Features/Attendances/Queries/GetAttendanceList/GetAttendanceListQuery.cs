using MediatR;

namespace BookLAB.Application.Features.Attendances.Queries.GetAttendanceList
{
    public record GetAttendanceListQuery(Guid ScheduleId) : IRequest<List<AttendanceStudentDto>>;
}
