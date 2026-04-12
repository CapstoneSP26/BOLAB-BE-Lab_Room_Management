using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookLAB.Infrastructure.Hubs
{

    [Authorize]
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("Id")?.Value;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroup(userId));
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("Id")?.Value;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroup(userId));
            }

            await base.OnDisconnectedAsync(exception);
        }

        public static string GetUserGroup(string userId) => $"user-notifications:{userId}";
    }
}
