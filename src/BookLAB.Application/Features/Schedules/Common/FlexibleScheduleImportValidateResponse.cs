using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Common
{
    public class FlexibleScheduleImportValidateResponse
    {
        public ImportMaps maps { get; set; }
        public ImportValidationResult<FlexibleScheduleImportDto, Schedule> result { get; set; }
    }
}
