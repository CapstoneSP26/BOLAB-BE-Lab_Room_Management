namespace BookLAB.Application.Common.Models;

public record CalendarEventDto
{
    public string EventId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string Location { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AttendeeEmail { get; init; } = string.Empty;
}
