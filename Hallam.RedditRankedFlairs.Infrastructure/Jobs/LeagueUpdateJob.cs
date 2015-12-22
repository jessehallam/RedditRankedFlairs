using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Riot;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Jobs
{
    /// <summary>
    ///     Updates the league standing for a summoner.
    /// </summary>
    public class LeagueUpdateJob
    {
        private readonly IRiotService _riot;
        private readonly ISummonerService _summoners;
        private readonly IUserService _users;

        public LeagueUpdateJob(IUserService users, IRiotService riot, ISummonerService summoners)
        {
            _users = users;
            _riot = riot;
            _summoners = summoners;
        }

        public void Execute(int userId, int summonerId)
        {
            var task = ExecuteInternalAsync(userId, summonerId);
            task.Wait();
        }

        private async Task ExecuteInternalAsync(int userId, int summonerId)
        {
            var user = await _users.FindAsync(userId);
            var summoner = user?.Summoners.FirstOrDefault(s => s.Id == summonerId);

            if (summoner == null) return;

            var leagues = await _riot.GetLeaguesAsync(summoner.Region, summoner.SummonerId);
            var solo = leagues?.FirstOrDefault(l => l.Queue == QueueType.RANKED_SOLO_5x5);

            if (solo != null)
            {
                var id = summoner.SummonerId.ToString();
                var entry = solo.Entries.FirstOrDefault(e => e.PlayerOrTeamId == id);

                if (entry != null)
                {
                    await _summoners.UpdateLeagueAsync(summoner, 
                        ParseTierName(solo.Tier), 
                        ParseDivision(entry.Division));
                    return;
                }
            }
            await _summoners.UpdateLeagueAsync(summoner, TierName.Unranked, 0);
        }

        private static int ParseDivision(DivisionType division)
        {
            return (int) division;
        }

        private static TierName ParseTierName(TierType tier)
        {
            return (TierName) Enum.Parse(typeof (TierName), tier.ToString(), true);
        }
    }
}