using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ValidateImport
{
    public class ValidateImportQuery : IRequest<ImportValidationResult<ScheduleImportDto, Schedule>>
    {
        public List<ScheduleImportDto> Schedules { get; set; } = new();
        public int CampusId { get; set; }
    }
}
