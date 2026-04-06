using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookLAB.Infrastructure.Services;

public class GoogleCalendarSyncService : ICalendarSyncService, IDisposable
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleCalendarSyncService> _logger;
    private readonly IHostEnvironment _environment;
    private readonly string _calendarId;
    private readonly string _timeZone;
    private CalendarService? _calendarService;
    private bool _disposed;

    public GoogleCalendarSyncService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<GoogleCalendarSyncService> logger,
        IHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
        _environment = environment;
        _calendarId = _configuration["GoogleCalendar:CalendarId"] ?? "primary";
        _timeZone = _configuration["GoogleCalendar:TimeZone"] ?? "Asia/Ho_Chi_Minh";
    }

    public async Task<string> CreateCalendarEventAsync(Guid scheduleId, CancellationToken cancellationToken)
    {
        var schedule = await _unitOfWork.Repository<Schedule>().Entities
            .Include(s => s.LabRoom)
                .ThenInclude(r => r.Building)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

        if (schedule == null)
            throw new InvalidOperationException($"Schedule {scheduleId} not found.");

        var service = await GetCalendarServiceAsync(cancellationToken);

        var eventTitle = $"Schedule - {schedule.LabRoom?.RoomName ?? "Unknown Room"} - {schedule.User?.FullName ?? "Unknown Lecturer"}";
        var location = schedule.LabRoom?.Building != null
            ? $"{schedule.LabRoom.RoomName}, {schedule.LabRoom.Building.BuildingName}"
            : schedule.LabRoom?.RoomName ?? "Unknown Location";

        var calendarEvent = new Event
        {
            Summary = eventTitle,
            Location = location,
            Description = BuildEventDescription(schedule),
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(schedule.StartTime.DateTime),
                TimeZone = _timeZone
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(schedule.EndTime.DateTime),
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

            _logger.LogInformation("Created calendar event {EventId} for schedule {ScheduleId}",
                createdEvent.Id, scheduleId);

            return createdEvent.Id;
        }
        catch (GoogleApiException gex)
        {
            _logger.LogError(gex,
                "Google Calendar API error while creating event for schedule {ScheduleId}. StatusCode: {StatusCode}, Message: {Message}",
                scheduleId, gex.HttpStatusCode, gex.Message);

            throw new InvalidOperationException("Google Calendar API error while creating event.", gex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create calendar event for schedule {ScheduleId}", scheduleId);
            throw new InvalidOperationException("Failed to sync schedule to Google Calendar.", ex);
        }
    }

    public async Task UpdateCalendarEventAsync(Guid scheduleId, string calendarEventId, CancellationToken cancellationToken)
    {
        var schedule = await _unitOfWork.Repository<Schedule>().Entities
            .Include(s => s.LabRoom)
                .ThenInclude(r => r.Building)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

        if (schedule == null)
            throw new InvalidOperationException($"Schedule {scheduleId} not found.");

        var service = await GetCalendarServiceAsync(cancellationToken);

        try
        {
            var existingEvent = await service.Events.Get(_calendarId, calendarEventId).ExecuteAsync(cancellationToken);

            var eventTitle = $"Schedule - {schedule.LabRoom?.RoomName ?? "Unknown Room"} - {schedule.User?.FullName ?? "Unknown Lecturer"}";
            var location = schedule.LabRoom?.Building != null
                ? $"{schedule.LabRoom.RoomName}, {schedule.LabRoom.Building.BuildingName}"
                : schedule.LabRoom?.RoomName ?? "Unknown Location";

            existingEvent.Summary = eventTitle;
            existingEvent.Location = location;
            existingEvent.Description = BuildEventDescription(schedule);
            existingEvent.Start = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(schedule.StartTime.DateTime),
                TimeZone = _timeZone
            };
            existingEvent.End = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(schedule.EndTime.DateTime),
                TimeZone = _timeZone
            };

            var updateRequest = service.Events.Update(existingEvent, _calendarId, calendarEventId);
            await updateRequest.ExecuteAsync(cancellationToken);

            _logger.LogInformation("Updated calendar event {EventId} for schedule {ScheduleId}",
                calendarEventId, scheduleId);
        }
        catch (GoogleApiException gex)
        {
            _logger.LogError(gex,
                "Google Calendar API error while updating event {EventId} for schedule {ScheduleId}. StatusCode: {StatusCode}, Message: {Message}",
                calendarEventId, scheduleId, gex.HttpStatusCode, gex.Message);

            throw new InvalidOperationException("Google Calendar API error while updating event.", gex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar event {EventId} for schedule {ScheduleId}",
                calendarEventId, scheduleId);
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

        // Handle relative paths by combining with content root
        if (!Path.IsPathRooted(credentialPath))
        {
            credentialPath = Path.Combine(_environment.ContentRootPath, credentialPath);
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

    private static string BuildEventDescription(Schedule schedule)
    {
        return $"""
                Schedule Details:
                - Type: {schedule.ScheduleType}
                - Status: {schedule.ScheduleStatus}
                - Schedule ID: {schedule.Id}
                
                This is an automated schedule from BookLAB system.
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
