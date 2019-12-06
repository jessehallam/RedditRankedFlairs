using System.Linq;
using System.Reflection;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace RedditFlairs.Core.Tasks
{
    public static class TaskExtensions
    {
        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            var taskTypes = from type in Assembly.GetExecutingAssembly().GetTypes()
                where type.IsClass
                      && typeof(IAsyncTask).IsAssignableFrom(type)
                select type;

            foreach (var type in taskTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }

        public static void EnableTask<TTask>(bool enable) where TTask : IAsyncTask
        {
            var taskName = typeof(TTask).Name;

            if (enable)
            {
                RecurringJob.AddOrUpdate<SingleInstanceTask<PersistentTask<TTask>>>(
                    taskName,
                    task => task.ExecuteAsync(null),
                    Cron.MinuteInterval(5));
            }
            else
            {
                RecurringJob.RemoveIfExists(taskName);
            }
        }
    }
}