using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Common
{
    public class ScheduleImportValidateResponse
    {
        public ImportMaps maps { get; set; }
        public ImportValidationResult<ScheduleImportDto, Schedule> result { get; set; }
    }
}
