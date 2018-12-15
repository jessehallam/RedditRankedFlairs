using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RedditFlairs.Core.Tasks
{
    public class PersistentTask<TTask> : IAsyncTask where TTask : IAsyncTask
    {
        private readonly IServiceProvider serviceProvider;

        public PersistentTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync()
        {
            while (true)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var asyncTask = scope.ServiceProvider.GetRequiredService<TTask>();
                    try
                    {
                        await asyncTask.ExecuteAsync();
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