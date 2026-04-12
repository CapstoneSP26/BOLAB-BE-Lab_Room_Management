
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
            DateTimeOffset StartTime,
            DateTimeOffset EndTime,
            CancellationToken ct,
            bool isAllowCreateImportData = false
        );
        Task<ImportValidationResult<FlexibleScheduleImportDto, Schedule>> ValidateFlexibleAsync(
            List<FlexibleScheduleImportDto> schedules,
            int campusId,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime,
            CancellationToken ct,
            bool isAllowCreateImportData = false
        );
        string GenerateHash(ScheduleImportDto d);
    }
}
