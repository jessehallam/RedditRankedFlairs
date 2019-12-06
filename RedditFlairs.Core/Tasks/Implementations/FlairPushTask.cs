using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedditFlairs.Core.Clients;
using RedditFlairs.Core.Clients.Reddit;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Core.Tasks.Implementations
{
    public class FlairPushTask : IAsyncTask
    {
        private readonly FlairPushConfig config;
        private readonly IServiceScope scope;
        private readonly IRedditClient redditClient;

        public FlairPushTask(IServiceScope scope, IRedditClient redditClient, IOptions<FlairPushConfig> config)
        {
            this.scope = scope;
            this.redditClient = redditClient;
            this.config = config.Value;
        }

        public async Task ExecuteAsync(PerformContext perform)
        {
            if (!config.Enable)
            {
               throw new TaskAbortedException();
            }

            using (var context = scope.ServiceProvider.GetRequiredService<FlairDbContext>())
            {
                var subReddits = context.SubReddits.ToList();
                var processors = subReddits.Select(x => new SubRedditProcessor(scope.ServiceProvider.CreateScope(), x.Name)).ToList();
                var processingTasks = processors.Select(x => x.ExecuteAsync(perform)).ToList();

                await Task.WhenAll(processingTasks);
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private class SubRedditProcessor : IAsyncTask
        {
            private readonly IRedditClient redditClient;
            private readonly IServiceScope scope;
            private readonly string subRedditName;

            public SubRedditProcessor(IServiceScope scope, string subRedditName)
            {
                this.scope = scope;
                this.subRedditName = subRedditName;

                redditClient = scope.ServiceProvider.GetRequiredService<IRedditClient>();
            }

            public async Task ExecuteAsync(PerformContext perform)
            {
                using (var context = scope.ServiceProvider.GetRequiredService<FlairDbContext>())
                {
                    var candidates = await GetCandidatesAsync(context);

                    if (!candidates.Any()) return;

                    var flairs = candidates.Select(uf => new FlairParameter
                    {
                        CssClass = uf.CssText,
                        Name = uf.User.Name,
                        Text = uf.Text
                    }).ToList();

                    await redditClient.SetFlairsAsync(subRedditName, flairs);

                    foreach (var flair in candidates)
                        flair.NeedToSend = false;

                    await context.SaveChangesAsync();
                }
            }

            private async Task<ICollection<UserFlair>> GetCandidatesAsync(FlairDbContext context)
            {
                var query =
                    from userFlair in context.UserFlairs.Include(uf => uf.User)
                    where userFlair.NeedToSend
                          && userFlair.SubReddit.Name == subRedditName
                    orderby userFlair.Id
                    select userFlair;

                return await query.Take(100).ToListAsync();
            }
        }
    }
}