namespace BookLAB.Application.Features.IncidentReports.Queries.GetUnresolvedIncidents
{
    public class IncidentDto
    {
        public Guid IncidentId { get; set; }
        public string LabRoomName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int DaysOpenCount { get; set; }
    }
}
