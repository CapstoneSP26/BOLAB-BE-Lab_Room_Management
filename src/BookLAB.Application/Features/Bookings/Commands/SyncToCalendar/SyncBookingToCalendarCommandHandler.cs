using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;

public class SyncBookingToCalendarCommandHandler : IRequestHandler<SyncBookingToCalendarCommand, string>
{
    private readonly IBookLABDbContext _context;
    private readonly ICalendarSyncService _calendarSyncService;
    private readonly ILogger<SyncBookingToCalendarCommandHandler> _logger;

    public SyncBookingToCalendarCommandHandler(
        IBookLABDbContext context,
        ICalendarSyncService calendarSyncService,
        ILogger<SyncBookingToCalendarCommandHandler> logger)
    {
        _context = context;
        _calendarSyncService = calendarSyncService;
        _logger = logger;
    }

    public async Task<string> Handle(SyncBookingToCalendarCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .Include(b => b.LabRoom)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for calendar sync", request.BookingId);
            throw new InvalidOperationException($"Booking with ID {request.BookingId} not found.");
        }

        if (booking.BookingStatus != BookingStatus.Approved)
        {
            _logger.LogWarning("Booking {BookingId} is not approved. Current status: {Status}", 
                request.BookingId, booking.BookingStatus);
            throw new InvalidOperationException("Only approved bookings can be synced to calendar.");
        }

        try
        {
            var calendarEventId = await _calendarSyncService.CreateCalendarEventAsync(
                request.BookingId,
                cancellationToken);

            booking.CalendarEventId = calendarEventId;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Booking {BookingId} synced to calendar with event ID {EventId}",
                request.BookingId, calendarEventId);

            return calendarEventId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync booking {BookingId} to calendar", request.BookingId);
            throw;
        }
    }
}
