using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;

public class SyncBookingToCalendarCommandHandler : IRequestHandler<SyncBookingToCalendarCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalendarSyncService _calendarSyncService;
    private readonly ILogger<SyncBookingToCalendarCommandHandler> _logger;

    public SyncBookingToCalendarCommandHandler(
        IUnitOfWork unitOfWork,
        ICalendarSyncService calendarSyncService,
        ILogger<SyncBookingToCalendarCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _calendarSyncService = calendarSyncService;
        _logger = logger;
    }

    public async Task<string> Handle(SyncBookingToCalendarCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().Entities
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
            if (!booking.ScheduleId.HasValue)
            {
                _logger.LogWarning("Booking {BookingId} has no associated schedule for calendar sync", request.BookingId);
                throw new InvalidOperationException("Booking must be associated with a schedule before syncing to calendar.");
            }

            var scheduleId = booking.ScheduleId.Value;

            var calendarEventId = await _calendarSyncService.CreateCalendarEventAsync(
                scheduleId,
                cancellationToken);

            var schedule = await _unitOfWork.Repository<Schedule>().GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                _logger.LogWarning("Schedule {ScheduleId} not found when syncing booking {BookingId} to calendar", scheduleId, request.BookingId);
                throw new InvalidOperationException($"Schedule with ID {scheduleId} not found.");
            }

            schedule.CalendarEventId = calendarEventId;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Booking {BookingId} synced to calendar via schedule {ScheduleId} with event ID {EventId}",
                request.BookingId, scheduleId, calendarEventId);

            return calendarEventId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync booking {BookingId} to calendar", request.BookingId);
            throw;
        }
    }
}
