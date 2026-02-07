namespace BookLAB.Application.Common.Interfaces.Services;

public interface ICalendarSyncService
{
    Task<string> CreateCalendarEventAsync(Guid bookingId, CancellationToken cancellationToken);
    Task UpdateCalendarEventAsync(Guid bookingId, string calendarEventId, CancellationToken cancellationToken);
    Task DeleteCalendarEventAsync(string calendarEventId, CancellationToken cancellationToken);
}
