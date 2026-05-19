using Hangfire.Dashboard;

namespace BookLAB.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var secret = httpContext.Request.Query["key"];

            return secret == "booklab-admin-hangfire-20052026";
        }
    }
}
