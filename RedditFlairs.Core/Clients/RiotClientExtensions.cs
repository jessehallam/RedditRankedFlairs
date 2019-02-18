using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditFlairs.Core.Clients.Reddit;

namespace RedditFlairs.Core.Clients
{
    public static class RiotClientExtensions
    {
        public static void AddRiotClient(this IServiceCollection services, IConfiguration configuration)
        {
            var cache = services.BuildServiceProvider().GetRequiredService<IMemoryCache>();
            var regionEndpionts = new Dictionary<string, string>();
            configuration.GetSection("Riot:RegionEndpoints").Bind(regionEndpionts);
            services.AddSingleton(new RiotClient(cache, regionEndpionts, configuration["Riot:ApiKey"]));
        }
    }

    public static class RedditClientExtensions
    {
        public static void AddRedditClient(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Reddit:API");
            var options = new RedditClientOptions
            {
                ClientId = config["ClientId"],
                ClientSecret = config["ClientSecret"],
                Password = config["Password"],
                UserAgent = "RedditRankedFlairs (v11) (by kivinkujata)",
                Username = config["Username"]
            };
            var client = new RedditClient(options) as IRedditClient;
            services.AddSingleton(client);
        }
    }
}