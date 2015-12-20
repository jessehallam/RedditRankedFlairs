using System.Web.Http;
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
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutofacConfig.Register(new ContainerBuilder());
        }
    }
}
