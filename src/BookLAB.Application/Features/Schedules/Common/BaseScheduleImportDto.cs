namespace BookLAB.Application.Features.Schedules.Common
{
    public class BaseScheduleImportDto
    {
        public int Index { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
        public bool IsUpdated { get; set; } = false;
        public bool IsValid { get; set; } = true;
    }
}
