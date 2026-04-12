using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ValidateImport
{
    public class ValidateFlexibleImportQuery : IRequest<ImportValidationResult<FlexibleScheduleImportDto, Schedule>>
    {
        public List<FlexibleScheduleImportDto> Schedules { get; set; } = new();
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int CampusId { get; set; }
    }
}
