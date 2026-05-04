
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IScheduleImportService
    {
        Task<ScheduleImportValidateResponse> ValidateAsync(
            List<ScheduleImportDto> schedules,
            int campusId,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime,
            Guid? ImportBatchId,
            CancellationToken ct,
            bool isAllowCreateImportData = false
        );
        Task<FlexibleScheduleImportValidateResponse> ValidateFlexibleAsync(
            List<FlexibleScheduleImportDto> schedules,
            int campusId,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime,
            Guid? ImportBatchId,
            CancellationToken ct,
            bool isAllowCreateImportData = false
        );
        string GenerateHash(ScheduleImportDto d);
    }
}
