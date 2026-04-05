namespace BookLAB.Application.Features.Schedules.Common
{
    public class FlexibleScheduleImportDto : BaseScheduleImportDto
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;

    }
}
