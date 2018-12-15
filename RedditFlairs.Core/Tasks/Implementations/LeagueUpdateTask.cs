using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedditFlairs.Core.Clients;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Core.Tasks.Implementations
{
    public class LeagueUpdateTask : IAsyncTask
    {
        private readonly LeagueUpdateConfig config;
        private readonly FlairDbContext context;
        private readonly RiotClient riotClient;

        public LeagueUpdateTask(IServiceScope scope, IOptions<LeagueUpdateConfig> config)
        {
            context = scope.ServiceProvider.GetRequiredService<FlairDbContext>();
            riotClient = scope.ServiceProvider.GetRequiredService<RiotClient>();
            this.config = config.Value;
        }

        public async Task ExecuteAsync()
        {
            if (!config.Enable)
            {
               throw new TaskAbortedException();
            }

            var candidates = (await GetCandidatesAsync()).ToList();

            if (!candidates.Any())
            {
                await Task.Delay(config.ExecutionInterval);
                return;
            }

            foreach (var summoner in candidates)
            {
                await UpdateSummonerAsync(summoner);
            }
            
            await context.SaveChangesAsync();
        }

        private async Task<IEnumerable<Summoner>> GetCandidatesAsync()
        {
            var cutoff = DateTimeOffset.Now.Subtract(config.UpdateInterval);

            var query = from summoner in context.Summoners
                where summoner.Validation.Status == ValidationStatus.Valid
                      && (summoner.RankUpdatedAt == null
                          || summoner.RankUpdatedAt < cutoff)
                orderby summoner.RankUpdatedAt
                select summoner;

            return await query.Take(config.MaximumCandidates).ToListAsync();
        }

        private async Task UpdateSummonerAsync(Summoner summoner)
        {
            var leaguePositions = await riotClient.GetLeaguePositionsAsync(summoner.Region, summoner.SummonerId);
            summoner.LeaguePositions.Clear();

            foreach (var leaguePos in leaguePositions)
            {
                summoner.LeaguePositions.Add(new LeaguePosition
                {
                    QueueType = leaguePos.QueueType,
                    Rank = leaguePos.Rank,
                    Tier = leaguePos.Tier
                });
            }

            summoner.RankUpdatedAt = DateTimeOffset.Now;
        }
    }
}