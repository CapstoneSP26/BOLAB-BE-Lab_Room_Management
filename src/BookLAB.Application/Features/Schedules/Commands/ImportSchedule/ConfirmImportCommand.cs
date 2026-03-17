using BookLAB.Application.Features.Schedules.Common;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmImportCommand : IRequest<bool>
    {
        public List<ScheduleImportDto> ValidSchedules { get; set; } = new();
    }
}