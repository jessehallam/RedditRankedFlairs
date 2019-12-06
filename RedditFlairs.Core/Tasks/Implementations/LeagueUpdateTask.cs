using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;
using RiotNet;
using RiotClient = RedditFlairs.Core.Clients.RiotClient;

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

        public async Task ExecuteAsync(PerformContext perform)
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
                try
                {
                    await UpdateSummonerAsync(summoner);
                }
                catch (RestException e)
                {
                    perform.WriteLine(
                        $"Error updating summoner: Id={summoner.Id} Region={summoner.Region} SummonerId={summoner.SummonerId}");
                    perform.LogHttpResponse(e.Response.Response);
                    perform.ResetTextColor();
                    throw;
                }
                catch (Exception e)
                {
                    perform.WriteLine(
                        $"Error updating summoner: Id={summoner.Id} Region={summoner.Region} SummonerId={summoner.SummonerId}");
                    perform.ResetTextColor();
                    throw;
                }
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

    internal static class LoggingExtensions
    {
        public static void LogHttpResponse(this PerformContext perform, HttpResponseMessage response)
        {
            var request = response.RequestMessage;
            var builder = new StringBuilder();

            builder.AppendLine("Request:");
            builder.AppendLine("  RequestUri = " + request.RequestUri);
            builder.AppendLine("  Headers:");

            foreach (var header in request.Headers)
            {
                foreach (var value in header.Value)
                {
                    builder.AppendLine($"    {header.Key}={value}");
                }
            }

            builder.AppendLine();
            builder.AppendLine("Response:");
            builder.AppendLine($"  StatusCode={response.StatusCode}");
            builder.AppendLine($"  ReasonPhrase={response.ReasonPhrase}");
            builder.AppendLine($"  Headers:");

            foreach (var header in response.Headers)
            {
                foreach (var value in header.Value)
                {
                    builder.AppendLine($"    {header.Key}={value}");
                }
            }

            builder.AppendLine("  Content:");
            builder.AppendLine("----");
            builder.AppendLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            builder.AppendLine("----");

            perform.WriteLine(builder.ToString());
        }
    }
}