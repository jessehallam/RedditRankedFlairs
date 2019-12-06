using System;
using System.Threading.Tasks;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;

namespace RedditFlairs.Core.Tasks
{
    public class SingleInstanceTask<TTask> : IAsyncTask where TTask : IAsyncTask
    {
        private readonly IServiceProvider serviceProvider;

        public SingleInstanceTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(PerformContext perform)
        {
            var locker = SingleInstanceRegistry.Instance.GetExclusiveLock(typeof(TTask));

            if (!locker.WaitOne(1000))
                return;

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var asyncTask = scope.ServiceProvider.GetRequiredService<TTask>();
                    await asyncTask.ExecuteAsync(perform);
                }
            }
            finally
            {
                locker.Release();
            }
        }
    }
}