using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.RejectBooking
{
    public record RejectBookingCommand(Guid BookingId, string? Reason) : IRequest<bool>;
}
