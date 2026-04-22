using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Common
{
    public class ImportMaps
    {
        public Dictionary<string, SlotType> SlotTypeMap { get; set; } = new();

        public Dictionary<string, LabRoom> RoomMap { get; set; } = new();

        public Dictionary<string, User> LecturerMap { get; set; } = new();

        public Dictionary<string, Group> GroupMap { get; set; } = new();

        public List<Schedule> ExistingSchedules { get; set; } = new();

        // ⚡ Optional (performance optimization)
        public HashSet<string> ExistingHashes { get; set; } = new();
        public HashSet<string> SubjectCodeHashes {  get; set; } = new();
    }
}