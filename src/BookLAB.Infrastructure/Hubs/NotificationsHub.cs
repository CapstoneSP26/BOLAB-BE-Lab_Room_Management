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
            var role = Context.User?.FindFirst("Role")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroup(userId));

                if (role == "1")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GetDashboardAdminGroup());
                }

                if (role == "2")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GetDashboardLabManagerGroup(userId));
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("Id")?.Value;
            var role = Context.User?.FindFirst("Role")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroup(userId));

                if (role == "1")
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetDashboardAdminGroup());
                }

                if (role == "2")
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetDashboardLabManagerGroup(userId));
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public static string GetUserGroup(string userId) => $"user-notifications:{userId}";
        public static string GetDashboardAdminGroup() => "dashboard:admin";
        public static string GetDashboardLabManagerGroup(string userId) => $"dashboard:labmanager:{userId}";
    }
}
