using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;

public class UpdateCalendarEventCommandHandler : IRequestHandler<UpdateCalendarEventCommand, Unit>
{
    private readonly IBookLABDbContext _context;
    private readonly ICalendarSyncService _calendarSyncService;
    private readonly ILogger<UpdateCalendarEventCommandHandler> _logger;

    public UpdateCalendarEventCommandHandler(
        IBookLABDbContext context,
        ICalendarSyncService calendarSyncService,
        ILogger<UpdateCalendarEventCommandHandler> logger)
    {
        _context = context;
        _calendarSyncService = calendarSyncService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for calendar update", request.BookingId);
            throw new InvalidOperationException($"Booking with ID {request.BookingId} not found.");
        }

        if (string.IsNullOrEmpty(booking.CalendarEventId))
        {
            _logger.LogWarning("Booking {BookingId} has no associated calendar event", request.BookingId);
            throw new InvalidOperationException("Booking has no associated calendar event.");
        }

        try
        {
            await _calendarSyncService.UpdateCalendarEventAsync(
                request.BookingId,
                booking.CalendarEventId,
                cancellationToken);

            _logger.LogInformation("Calendar event {EventId} updated for booking {BookingId}",
                booking.CalendarEventId, request.BookingId);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar event for booking {BookingId}", request.BookingId);
            throw;
        }
    }
}
