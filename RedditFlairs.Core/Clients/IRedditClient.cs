using System.Collections.Generic;
using System.Threading.Tasks;
using RedditFlairs.Core.Clients.Reddit;

namespace RedditFlairs.Core.Clients
{
    public interface IRedditClient
    {
        Task SetFlairsAsync(string sub, ICollection<FlairParameter> flairs);
    }
}