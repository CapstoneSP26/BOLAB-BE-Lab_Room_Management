using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;

public record DeleteCalendarEventCommand(Guid BookingId) : IRequest<Unit>;
