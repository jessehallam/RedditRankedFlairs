using System;
using System.Linq;
using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;

namespace RedditFlairs.Web
{
    public class DashboardAuthorize : IDashboardAuthorizationFilter
    {
        private readonly string[] authorizedUsers;

        public DashboardAuthorize(IConfiguration configuration)
        {
            authorizedUsers = configuration["Dashboard:AuthorizedUsers"].Split(',');
        }

        public bool Authorize(DashboardContext context)
        {
            return context.GetHttpContext().User.Identity.IsAuthenticated
                   && authorizedUsers.Contains(context.GetHttpContext().User.Identity.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
