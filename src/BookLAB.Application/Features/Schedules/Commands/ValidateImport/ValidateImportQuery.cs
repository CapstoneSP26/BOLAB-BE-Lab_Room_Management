using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ValidateImport
{
    public class ValidateImportQuery : IRequest<ImportValidationResult<ScheduleImportDto>>
    {
        public List<ScheduleImportDto> Schedules { get; set; } = new();
        public int CampusId { get; set; }
    }
}
