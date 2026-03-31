using MediatR;

namespace BookLAB.Application.Features.Bookings.Events
{
    public record BookingRejectedEvent(Guid BookingId) : INotification
    {
    }
}
