
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IScheduleImportService
    {
        Task<ImportValidationResult<ScheduleImportDto, Schedule>> ValidateAsync(
            List<ScheduleImportDto> schedules,
            int campusId,
            CancellationToken ct,
            bool isAllowCreateImportData = false
        );
        string GenerateHash(ScheduleImportDto d);
    }
}
