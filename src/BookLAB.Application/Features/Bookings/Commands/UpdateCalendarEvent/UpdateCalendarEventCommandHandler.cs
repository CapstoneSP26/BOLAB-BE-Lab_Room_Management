using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;

public class UpdateCalendarEventCommandHandler : IRequestHandler<UpdateCalendarEventCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalendarSyncService _calendarSyncService;
    private readonly ILogger<UpdateCalendarEventCommandHandler> _logger;

    public UpdateCalendarEventCommandHandler(
        IUnitOfWork unitOfWork,
        ICalendarSyncService calendarSyncService,
        ILogger<UpdateCalendarEventCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _calendarSyncService = calendarSyncService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found for calendar update", request.BookingId);
            throw new InvalidOperationException($"Booking with ID {request.BookingId} not found.");
        }

        if (!booking.ScheduleId.HasValue)
        {
            _logger.LogWarning("Booking {BookingId} has no associated schedule for calendar update", request.BookingId);
            throw new InvalidOperationException("Booking must be associated with a schedule before updating calendar event.");
        }

        var schedule = await _unitOfWork.Repository<Schedule>().GetByIdAsync(booking.ScheduleId.Value);

        if (schedule == null)
        {
            _logger.LogWarning("Schedule {ScheduleId} not found for booking {BookingId} calendar update", booking.ScheduleId, request.BookingId);
            throw new InvalidOperationException($"Schedule with ID {booking.ScheduleId} not found.");
        }

        if (string.IsNullOrEmpty(schedule.CalendarEventId))
        {
            _logger.LogWarning("Booking {BookingId} has no associated calendar event", request.BookingId);
            throw new InvalidOperationException("Booking has no associated calendar event.");
        }

        try
        {
            await _calendarSyncService.UpdateCalendarEventAsync(
                schedule.Id,
                schedule.CalendarEventId,
                cancellationToken);

            _logger.LogInformation("Calendar event {EventId} updated for booking {BookingId}",
                schedule.CalendarEventId, request.BookingId);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar event for booking {BookingId}", request.BookingId);
            throw;
        }
    }
}
