using Hangfire.Dashboard;

namespace BookLAB.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            {
                return false;
            }

            return httpContext.User.HasClaim("Role", "1");
        }
    }
}
