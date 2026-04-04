using MediatR;

namespace BookLAB.Application.Common.Events
{
    public record BookingApprovedEvent(Guid BookingId, Guid userId) : INotification;
}
