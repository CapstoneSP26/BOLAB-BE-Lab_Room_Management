namespace BookLAB.Application.Features.Schedules.Common
{
    public class ScheduleImportDto : BaseScheduleImportDto
    {
        public int SlotOrder { get; set; }
        public string SlotTypeCode { get; set; } = string.Empty;

    }
}
