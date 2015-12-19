using System.Web.Mvc;
using System.Web.Routing;
using Autofac;

namespace Hallam.RedditRankedFlairs
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutofacConfig.Register(new ContainerBuilder());
        }
    }
}
