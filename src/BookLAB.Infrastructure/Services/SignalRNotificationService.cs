using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BookLAB.Infrastructure.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNotificationCreatedAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group(NotificationsHub.GetUserGroup(userId.ToString()))
                .SendAsync("notification.created", payload, cancellationToken);
        }

        public async Task NotifyBookingChangedAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group(NotificationsHub.GetUserGroup(userId.ToString()))
                .SendAsync("booking.changed", payload, cancellationToken);
        }
    }
}