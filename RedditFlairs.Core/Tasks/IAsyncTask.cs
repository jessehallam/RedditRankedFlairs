using System.Threading.Tasks;
using Hangfire.Server;

namespace RedditFlairs.Core.Tasks
{
    public interface IAsyncTask
    {
        Task ExecuteAsync(PerformContext perform);
    }
}