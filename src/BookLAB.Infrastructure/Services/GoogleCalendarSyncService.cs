using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Application.Common.Interfaces.Services;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookLAB.Infrastructure.Services;

public class GoogleCalendarSyncService : ICalendarSyncService, IDisposable
{
    private readonly IBookLABDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleCalendarSyncService> _logger;
    private readonly string _calendarId;
    private readonly string _timeZone;
    private CalendarService? _calendarService;
    private bool _disposed;

    public GoogleCalendarSyncService(
        IBookLABDbContext context,
        IConfiguration configuration,
        ILogger<GoogleCalendarSyncService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _calendarId = _configuration["GoogleCalendar:CalendarId"] ?? "primary";
        _timeZone = _configuration["GoogleCalendar:TimeZone"] ?? "Asia/Ho_Chi_Minh";
    }

    public async Task<string> CreateCalendarEventAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .Include(b => b.LabRoom)
                .ThenInclude(r => r.Building)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

        if (booking == null)
            throw new InvalidOperationException($"Booking {bookingId} not found.");

        var service = await GetCalendarServiceAsync(cancellationToken);

        var eventTitle = $"Lab Booking - {booking.LabRoom?.RoomName ?? "Unknown Room"}";
        var location = booking.LabRoom?.Building != null
            ? $"{booking.LabRoom.RoomName}, {booking.LabRoom.Building.BuildingName}"
            : booking.LabRoom?.RoomName ?? "Unknown Location";

        var calendarEvent = new Event
        {
            Summary = eventTitle,
            Location = location,
            Description = BuildEventDescription(booking),
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(booking.StartTime),
                TimeZone = _timeZone
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(booking.EndTime),
                TimeZone = _timeZone
            },
            Reminders = new Event.RemindersData
            {
                UseDefault = false,
                Overrides = new List<EventReminder>
                {
                    new() { Method = "email", Minutes = 24 * 60 }, // 1 day before
                    new() { Method = "popup", Minutes = 30 }       // 30 minutes before
                }
            }
        };

        try
        {
            var request = service.Events.Insert(calendarEvent, _calendarId);
            var createdEvent = await request.ExecuteAsync(cancellationToken);

            _logger.LogInformation("Created calendar event {EventId} for booking {BookingId}",
                createdEvent.Id, bookingId);

            return createdEvent.Id;
        }
        catch (GoogleApiException gex)
        {
            _logger.LogError(gex,
                "Google Calendar API error while creating event for booking {BookingId}. StatusCode: {StatusCode}, Message: {Message}",
                bookingId, gex.HttpStatusCode, gex.Message);

            throw new InvalidOperationException("Google Calendar API error while creating event.", gex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create calendar event for booking {BookingId}", bookingId);
            throw new InvalidOperationException("Failed to sync booking to Google Calendar.", ex);
        }
    }

    public async Task UpdateCalendarEventAsync(Guid bookingId, string calendarEventId, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .Include(b => b.LabRoom)
                .ThenInclude(r => r.Building)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

        if (booking == null)
            throw new InvalidOperationException($"Booking {bookingId} not found.");

        var service = await GetCalendarServiceAsync(cancellationToken);

        try
        {
            var existingEvent = await service.Events.Get(_calendarId, calendarEventId).ExecuteAsync(cancellationToken);

            var eventTitle = $"Lab Booking - {booking.LabRoom?.RoomName ?? "Unknown Room"}";
            var location = booking.LabRoom?.Building != null
                ? $"{booking.LabRoom.RoomName}, {booking.LabRoom.Building.BuildingName}"
                : booking.LabRoom?.RoomName ?? "Unknown Location";

            existingEvent.Summary = eventTitle;
            existingEvent.Location = location;
            existingEvent.Description = BuildEventDescription(booking);
            existingEvent.Start = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(booking.StartTime),
                TimeZone = _timeZone
            };
            existingEvent.End = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(booking.EndTime),
                TimeZone = _timeZone
            };

            var updateRequest = service.Events.Update(existingEvent, _calendarId, calendarEventId);
            await updateRequest.ExecuteAsync(cancellationToken);

            _logger.LogInformation("Updated calendar event {EventId} for booking {BookingId}",
                calendarEventId, bookingId);
        }
        catch (GoogleApiException gex)
        {
            _logger.LogError(gex,
                "Google Calendar API error while updating event {EventId} for booking {BookingId}. StatusCode: {StatusCode}, Message: {Message}",
                calendarEventId, bookingId, gex.HttpStatusCode, gex.Message);

            throw new InvalidOperationException("Google Calendar API error while updating event.", gex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar event {EventId} for booking {BookingId}",
                calendarEventId, bookingId);
            throw new InvalidOperationException("Failed to update calendar event.", ex);
        }
    }

    public async Task DeleteCalendarEventAsync(string calendarEventId, CancellationToken cancellationToken)
    {
        var service = await GetCalendarServiceAsync(cancellationToken);

        try
        {
            var deleteRequest = service.Events.Delete(_calendarId, calendarEventId);
            await deleteRequest.ExecuteAsync(cancellationToken);

            _logger.LogInformation("Deleted calendar event {EventId}", calendarEventId);
        }
        catch (GoogleApiException gex)
        {
            _logger.LogError(gex,
                "Google Calendar API error while deleting event {EventId}. StatusCode: {StatusCode}, Message: {Message}",
                calendarEventId, gex.HttpStatusCode, gex.Message);

            throw new InvalidOperationException("Google Calendar API error while deleting event.", gex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete calendar event {EventId}", calendarEventId);
            throw new InvalidOperationException("Failed to delete calendar event.", ex);
        }
    }

    private async Task<CalendarService> GetCalendarServiceAsync(CancellationToken cancellationToken)
    {
        var credentialPath = _configuration["GoogleCalendar:CredentialsPath"];

        if (string.IsNullOrEmpty(credentialPath))
        {
            _logger.LogError("Google Calendar credentials path not configured");
            throw new InvalidOperationException("Google Calendar credentials path not configured in appsettings.");
        }

        if (!File.Exists(credentialPath))
        {
            _logger.LogError("Google Calendar credentials file not found at {Path}", credentialPath);
            throw new FileNotFoundException($"Credentials file not found at: {credentialPath}");
        }

        GoogleCredential credential;

        using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream);
            credential = credential.CreateScoped(CalendarService.Scope.Calendar);
        }

        if (_calendarService != null)
        {
            return _calendarService;
        }

        _calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "BookLAB"
        });

        return _calendarService;
    }

    private static string BuildEventDescription(BookLAB.Domain.Entities.Booking booking)
    {
        return $"""
                Booking Details:
                - Reason: {booking.Reason ?? "N/A"}
                - Status: {booking.BookingStatus}
                - Booking ID: {booking.Id}
                
                This is an automated booking from BookLAB system.
                """;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _calendarService?.Dispose();
        _disposed = true;
    }
}
