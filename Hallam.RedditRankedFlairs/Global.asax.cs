using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Hallam.RedditRankedFlairs.Jobs;
using Hangfire;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace Hallam.RedditRankedFlairs
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutofacConfig.Register(new ContainerBuilder());
        }
    }
}
