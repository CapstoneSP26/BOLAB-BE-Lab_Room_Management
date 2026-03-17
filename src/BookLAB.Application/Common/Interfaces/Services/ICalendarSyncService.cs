namespace BookLAB.Application.Common.Interfaces.Services;

public interface ICalendarSyncService
{
    Task<string> CreateCalendarEventAsync(Guid scheduleId, CancellationToken cancellationToken);
    Task UpdateCalendarEventAsync(Guid scheduleId, string calendarEventId, CancellationToken cancellationToken);
    Task DeleteCalendarEventAsync(string calendarEventId, CancellationToken cancellationToken);
}
