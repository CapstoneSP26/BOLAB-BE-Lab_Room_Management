namespace BookLAB.Application.Features.Schedules.Common
{
    public class ConflictScheduleDto
    {
        public string Lecturer { get; set; }
        public string RoomNo { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public string Source { get; set; }
        public object RefId { get; set; }
        public int Index { get; set; }
    }
}
