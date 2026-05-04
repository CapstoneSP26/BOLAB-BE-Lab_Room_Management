using MediatR;

namespace BookLAB.Application.Features.Bookings.Events
{
    public record BookingApprovedEvent(Guid BookingId, Guid userId) : INotification;
}
