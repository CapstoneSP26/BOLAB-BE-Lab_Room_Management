using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ValidateImport
{
    public class ValidateFlexibleImportHandler : IRequestHandler<ValidateFlexibleImportQuery, ImportValidationResult<FlexibleScheduleImportDto, Schedule>>
    {
        private readonly IScheduleImportService _scheduleImportService;
        public ValidateFlexibleImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleService)
        {
            _scheduleImportService = scheduleService;
        }
        public async Task<ImportValidationResult<FlexibleScheduleImportDto, Schedule>> Handle(ValidateFlexibleImportQuery request, CancellationToken cancellationToken)
        {
            var result = await _scheduleImportService.ValidateFlexibleAsync(request.Schedules, request.CampusId, cancellationToken);
            return result;
        }
    }
}
