using System;
using System.Configuration;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Services;
using Hallam.RedditRankedFlairs.Services.Riot;

namespace Hallam.RedditRankedFlairs
{
    public class AutofacConfig
    {
        public static void Register(ContainerBuilder builder)
        {
            // MVC controllers
            builder.RegisterControllers(typeof (MvcApplication).Assembly);

            // DI model binders
            builder.RegisterModelBinders(typeof (MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // Web abstractions
            builder.RegisterModule<AutofacWebTypesModule>();
            
            // Property injection in view pages
            builder.RegisterSource(new ViewRegistrationSource());

            // Property injection in action filters
            builder.RegisterFilterProvider();

            // Services
            builder.RegisterType(typeof (RoleService)).As(typeof (IRoleService)).SingleInstance();
            builder.RegisterType(typeof (UserService)).As(typeof (IUserService)).InstancePerLifetimeScope();
            builder.RegisterType(typeof (SummonerService)).As(typeof (ISummonerService)).InstancePerLifetimeScope();
            builder.Register(context => new RiotService
            {
                WebRequester = new RiotWebRequester
                {
                    ApiKey = ConfigurationManager.AppSettings["riot.apiKey"],
                    MaxAttempts = int.Parse(ConfigurationManager.AppSettings["riot.maxAttempts"]),
                    MaxRequestsPer10Seconds = int.Parse(ConfigurationManager.AppSettings["riot.maxRequestsPer10Seconds"]),
                    RetryInterval = TimeSpan.Parse(ConfigurationManager.AppSettings["riot.retryInterval"])
                }
            }).As(typeof (IRiotService)).SingleInstance();

            // Data persistance
            builder.RegisterType(typeof (UnitOfWork)).As(typeof (IUnitOfWork)).InstancePerLifetimeScope();

            Configure(builder.Build());
        }

        private static void Configure(IContainer container)
        {
            // Replace the MVC dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}