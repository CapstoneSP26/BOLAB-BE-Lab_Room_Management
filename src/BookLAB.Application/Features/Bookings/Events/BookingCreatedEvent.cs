using MediatR;

namespace BookLAB.Application.Features.Bookings.Events
{
    public record BookingCreatedEvent(Guid BookingId) : INotification
    {
    }
}
