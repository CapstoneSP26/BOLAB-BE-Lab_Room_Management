using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;

public class DeleteCalendarEventCommandHandler : IRequestHandler<DeleteCalendarEventCommand, Unit>
{
    private readonly IBookLABDbContext _context;
    private readonly ICalendarSyncService _calendarSyncService;
    private readonly ILogger<DeleteCalendarEventCommandHandler> _logger;

    public DeleteCalendarEventCommandHandler(
        IBookLABDbContext context,
        ICalendarSyncService calendarSyncService,
        ILogger<DeleteCalendarEventCommandHandler> logger)
    {
        _context = context;
        _calendarSyncService = calendarSyncService;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for calendar deletion", request.BookingId);
            throw new InvalidOperationException($"Booking with ID {request.BookingId} not found.");
        }

        if (!booking.ScheduleId.HasValue)
        {
            _logger.LogInformation("Booking {BookingId} has no schedule associated; nothing to delete on calendar", request.BookingId);
            return Unit.Value;
        }

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.Id == booking.ScheduleId.Value, cancellationToken);

        if (schedule == null)
        {
            _logger.LogInformation("Schedule {ScheduleId} not found for booking {BookingId}; nothing to delete on calendar", booking.ScheduleId, request.BookingId);
            return Unit.Value;
        }

        if (string.IsNullOrEmpty(schedule.CalendarEventId))
        {
            _logger.LogInformation("Booking {BookingId} has no calendar event to delete", request.BookingId);
            return Unit.Value;
        }

        try
        {
            var calendarEventId = schedule.CalendarEventId;

            await _calendarSyncService.DeleteCalendarEventAsync(
                calendarEventId,
                cancellationToken);

            schedule.CalendarEventId = null;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Calendar event {EventId} deleted for booking {BookingId}",
                calendarEventId, request.BookingId);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete calendar event for booking {BookingId}", request.BookingId);
            throw;
        }
    }
}
