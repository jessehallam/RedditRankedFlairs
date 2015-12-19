using System.Web.Mvc;
using System.Web.Routing;

namespace Hallam.RedditRankedFlairs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Profile", "profile/{action}", new {controller = "Profile", action = "index"});
            routes.MapRoute("Login", "{action}", new {controller = "Login", action = "index"});
        }
    }
}
