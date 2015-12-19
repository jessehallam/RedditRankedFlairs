using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Services
{
    /// <summary>
    ///     Manages the summoners which are registered to users.
    /// </summary>
    public interface ISummonerService
    {
        /// <summary>
        ///     Finds a summoner asynchronously.
        /// </summary>
        /// <param name="region">The summoner region.</param>
        /// <param name="summonerName">The summoner name.</param>
        /// <returns>A summoner object.</returns>
        Task<Summoner> FindAsync(string region, string summonerName);

        /// <summary>
        ///     Determines if a summoner is registered asynchronously.
        /// </summary>
        /// <param name="region">The summoner region.</param>
        /// <param name="summonerName">The summoner name.</param>
        /// <returns>A value indicating whether the summoner is registered.</returns>
        Task<bool> IsSummonerRegistered(string region, string summonerName);

        /// <summary>
        ///     Removes a summoner from the repository asynchronously.
        /// </summary>
        /// <param name="region">The summoner region.</param>
        /// <param name="summonerName">The summoner name.</param>
        /// <returns>True indicates success.</returns>
        Task<bool> RemoveAsync(string region, string summonerName);
    }
}