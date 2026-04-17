using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmFlexibleImportCommand : IRequest<ImportResult>
    {
        public List<FlexibleScheduleImportDto> Schedules { get; set; } = new();
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int CampusId { get; set; }
    }
}