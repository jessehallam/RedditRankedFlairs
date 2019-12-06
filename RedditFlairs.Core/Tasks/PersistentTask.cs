using System;
using System.Threading.Tasks;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;

namespace RedditFlairs.Core.Tasks
{
    public class PersistentTask<TTask> : IAsyncTask where TTask : IAsyncTask
    {
        private static readonly TimeSpan MaximumRuntime = TimeSpan.FromMinutes(4.9);

        private readonly IServiceProvider serviceProvider;

        public PersistentTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(PerformContext perform)
        {
            var startTime = DateTimeOffset.UtcNow;

            while (true)
            {
                var now = DateTimeOffset.UtcNow;
                var runTime = now - startTime;

                // A new job instance will get started in moments after.
                // This fixes a "feature" of HangFire which causes it to hold open
                // a SQL transaction for the lifetime of the job, causing log files
                // to grow out of control.
                if (runTime > MaximumRuntime) break;

                using (var scope = serviceProvider.CreateScope())
                {
                    var asyncTask = scope.ServiceProvider.GetRequiredService<TTask>();
                    try
                    {
                        await asyncTask.ExecuteAsync(perform);
                    }
                    catch (TaskAbortedException)
                    {
                        return;
                    }
                }
            }
        }
    }
}