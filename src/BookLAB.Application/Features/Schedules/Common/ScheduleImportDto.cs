namespace BookLAB.Application.Features.Schedules.Common
{
    public class ScheduleImportDto
    {
        public int Index { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int SlotOrder { get; set; }
        public string SlotTypeCode { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
    }
}
