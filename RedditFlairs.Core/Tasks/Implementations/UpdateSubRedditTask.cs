using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedditFlairs.Core.Clients;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;

namespace RedditFlairs.Core.Tasks.Implementations
{
    public class UpdateSubRedditTask : IAsyncTask
    {
        private readonly IServiceScope scope;
        private readonly IRedditClient redditClient;
        private readonly FlairPushConfig config;

        public UpdateSubRedditTask(IServiceScope scope, IRedditClient redditClient, IOptions<FlairPushConfig> config)
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
                var subReddits = context.SubReddits.Where(sub => sub.RequiresUpdate == true).ToList();
                var processors = subReddits.Select(sub => new SubRedditProcessor(scope.ServiceProvider.CreateScope(), sub.Name)).ToList();
                var processingTasks = processors.Select(p => p.ExecuteAsync(perform)).ToList();

                await Task.WhenAll(processingTasks);
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private class SubRedditProcessor : IAsyncTask
        {
            private readonly IServiceScope scope;
            private readonly string subRedditName;
            private readonly IRedditClient redditClient;

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
                    var summoners = context.Summoners.ToList();

                }
            }
        }
    }
}