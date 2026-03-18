using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;

public record UpdateCalendarEventCommand(Guid BookingId) : IRequest<Unit>;
