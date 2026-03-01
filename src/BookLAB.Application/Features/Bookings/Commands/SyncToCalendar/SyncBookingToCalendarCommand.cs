using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;

public record SyncBookingToCalendarCommand(Guid BookingId) : IRequest<string>;
