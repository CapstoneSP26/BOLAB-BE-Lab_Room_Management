using MediatR;

namespace BookLAB.Application.Features.Profile.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommand : IRequest<Unit>
{
    public int NotificationId { get; set; }
}
