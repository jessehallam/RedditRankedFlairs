using System.Threading.Tasks;

namespace RedditFlairs.Core.Tasks
{
    public interface IAsyncTask
    {
        Task ExecuteAsync();
    }
}