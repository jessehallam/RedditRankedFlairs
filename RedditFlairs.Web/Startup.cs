using System;
using System.Collections.Generic;
using System.IO;
using AspNet.Security.OAuth.Reddit;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditFlairs.Core.Clients;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Tasks;
using RedditFlairs.Core.Tasks.Implementations;
using RedditFlairs.Core.Utility;

namespace RedditFlairs.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TasksConfigSection>(Configuration.GetSection("Tasks"));
            services.Configure<FlairUpdateConfig>(Configuration.GetSection("Tasks:FlairUpdate"));
            services.Configure<LeagueUpdateConfig>(Configuration.GetSection("Tasks:LeagueUpdate"));
            services.Configure<ValidationConfig>(Configuration.GetSection("Tasks:Validation"));

            services.AddTransient<IServiceScope>(provider => provider.CreateScope());
            services.AddScoped<RankUtility>();
            services.AddMemoryCache();

            services.AddDbContext<FlairDbContext>(config =>
            {
                var connectionString = Configuration.GetConnectionString("Default");
                config
                    .UseSqlServer(connectionString)
                    .UseLazyLoadingProxies();
            });

            services.AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = RedditAuthenticationDefaults.AuthenticationScheme;
                    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    config.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddReddit(config =>
                {
                    config.ClientId = Configuration["Reddit:SignIn:ClientId"];
                    config.ClientSecret = Configuration["Reddit:SignIn:ClientSecret"];
                });
            services.AddAuthorization();


            services.AddHangfire(config => { config.UseSqlServerStorage(Configuration.GetConnectionString("HangFire")); });
            services.AddMvc(config => { config.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>(); });

            services.AddTasks();

            services.AddRedditClient(Configuration);
            services.AddRiotClient(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    ConfigFile = Path.Combine(env.ContentRootPath, "ClientApp", "webpack.config.js"),
                    EnvironmentVariables = new Dictionary<string, string>()
                    {
                        {"NODE_ENV", "development"}
                    },
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }

            app.UseAuthentication();
            app.UseStaticFiles();

            if (Configuration.GetValue<bool>("Tasks:Enable"))
            {
                app.UseHangfireServer();
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new DashboardAuthorize(Configuration),
                    }
                });

                TaskExtensions.EnableTask<FlairUpdateTask>(Configuration.GetValue<bool>("Tasks:FlairUpdate:Enable"));
                TaskExtensions.EnableTask<LeagueUpdateTask>(Configuration.GetValue<bool>("Tasks:LeagueUpdate:Enable"));
                TaskExtensions.EnableTask<ValidationTask>(Configuration.GetValue<bool>("Tasks:Validation:Enable"));
                TaskExtensions.EnableTask<FlairPushTask>(Configuration.GetValue<bool>("Tasks:FlairPush:Enable"));
            }

            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.DefaultPage = "/dist/index.html";
                spa.Options.SourcePath = "ClientApp";
            });
        }
    }
}