using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiotNet;

namespace RedditFlairs.Core.Clients
{
    public class RiotClient
    {
        private readonly IMemoryCache cache;
        private readonly string apiKey;
        private readonly IDictionary<string, string> regionalEndpoints;

        private readonly IRiotClient ajrClient;

        public RiotClient(
            IMemoryCache cache,
            IDictionary<string, string> regionalEndpoints,
            string apiKey)
        {
            this.cache = cache;
            this.apiKey = apiKey;
            this.regionalEndpoints =
                regionalEndpoints.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

            ajrClient = new RiotNet.RiotClient(new RiotClientSettings
            {
                ApiKey = apiKey
            });
        }

        public Task<string> GetThirdPartyCodeAsync(string region, string encryptedSummonerId)
        {
            return cache.GetOrCreateAsync(
                $"ThirdPartyCode?{nameof(region)}={region}&{nameof(encryptedSummonerId)}={encryptedSummonerId}",
                entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(45));
                    return ajrClient.GetThirdPartyCodeAsync(encryptedSummonerId, GetPlatformId(region));
                });
        }

        public async Task<IEnumerable<LeaguePositionDto>> GetLeaguePositionsAsync(string region, string encryptedSummonerId)
        {
            var results = await cache.GetOrCreateAsync(
                $"LeaguePositions?{nameof(region)}={region}&{nameof(encryptedSummonerId)}={encryptedSummonerId}",
                entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1));
                    return ajrClient.GetLeaguePositionsBySummonerIdAsync(encryptedSummonerId, GetPlatformId(region));
                });

            return results.Select(pos => new LeaguePositionDto
            {
                QueueType = pos.QueueType,
                Rank = pos.Rank,
                Tier = pos.Tier
            });
        }

        public async Task<SummonerDto> GetSummonerAsync(string region, string summonerName)
        {
            var result = await cache.GetOrCreateAsync(
                $"Summoner?{nameof(region)}={region}&{nameof(summonerName)}={summonerName}",
                entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(4));
                    return ajrClient.GetSummonerBySummonerNameAsync(summonerName, GetPlatformId(region));
                });

            return result == null
                ? null
                : new SummonerDto
                {
                    AccountId = result.AccountId,
                    Id = result.Id,
                    Name = result.Name,
                    PuuId = result.Puuid
                };
        }

        private string GetPlatformId(string region)
        {
            if (!regionalEndpoints.TryGetValue(region, out var platformId))
                throw new InvalidOperationException("Unrecognized region: " + region);

            return platformId;
        }
    }

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

    public class SummonerDto
    {
        public string AccountId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string PuuId { get; set; }
    }

    public class LeaguePositionDto
    {
        public string QueueType { get; set; }
        public string Rank { get; set; }
        public string Tier { get; set; }
    }
}