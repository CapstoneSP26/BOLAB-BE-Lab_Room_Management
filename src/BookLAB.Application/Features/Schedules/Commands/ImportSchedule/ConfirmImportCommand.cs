using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmImportCommand : IRequest<ImportResult>
    {
        public List<ScheduleImportDto> Schedules { get; set; } = new();
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int CampusId { get; set; }
    }
}