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
    public class ValidationTask : IAsyncTask
    {
        private readonly ValidationConfig config;
        private readonly FlairDbContext context;
        private readonly RiotClient riotClient;
        private readonly IServiceScope scope;

        public ValidationTask(IServiceScope scope, IOptions<ValidationConfig> config)
        {
            this.scope = scope;
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
                await ValidateSummonerAsync(summoner);
            }
            
            await context.SaveChangesAsync();
        }

        private async Task<IEnumerable<Summoner>> GetCandidatesAsync()
        {
            var cutoff = DateTimeOffset.Now.Subtract(config.AttemptInterval);

            var query = from summoner in context.Summoners
                where summoner.Validation.Status == ValidationStatus.NotValidated
                      && (summoner.Validation.AttemptedAt == null
                          || summoner.Validation.AttemptedAt < cutoff)
                orderby summoner.Validation.AttemptedAt
                select summoner;

            return await query.Take(config.MaximumCandidates).ToListAsync();
        }

        private async Task ValidateSummonerAsync(Summoner summoner)
        {
            var thirdPartyCode = await riotClient.GetThirdPartyCodeAsync(summoner.Region, summoner.SummonerId);

            summoner.Validation.AttemptedAt = DateTimeOffset.Now;

            if (summoner.Validation.Code.Equals(thirdPartyCode, StringComparison.InvariantCultureIgnoreCase))
            {
                summoner.Validation.Status = ValidationStatus.Valid;
            }
            else
            {
                summoner.Validation.Attempts++;
                if (summoner.Validation.Attempts >= config.MaximumAttempts)
                {
                    summoner.Validation.Status = ValidationStatus.Failed;
                }
            }
        }
    }
}