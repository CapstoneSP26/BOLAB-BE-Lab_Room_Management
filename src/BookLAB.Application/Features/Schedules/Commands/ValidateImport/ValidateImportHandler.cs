using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ValidateImport
{
    public class ValidateImportHandler : IRequestHandler<ValidateImportQuery, ImportValidationResult<ScheduleImportDto, Schedule>>
    {
        private readonly IScheduleImportService _scheduleImportService;
        public ValidateImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleService)
        {
            _scheduleImportService = scheduleService;
        }
        public async Task<ImportValidationResult<ScheduleImportDto, Schedule>> Handle(ValidateImportQuery request, CancellationToken cancellationToken)
        {
            var result = await _scheduleImportService.ValidateAsync(request.Schedules, request.CampusId, request.StartTime, request.EndTime, cancellationToken);
            return result;
        }
    }
}
