using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Riot;
using Hallam.RedditRankedFlairs.Services.Riot;

namespace Hallam.RedditRankedFlairs.Services
{
    public class RiotService : IRiotService
    {
        private const string LeagueBaseUri = "v2.5/league/";
        private const string SummonerBaseUri = "v1.4/summoner/";

        public RiotWebRequester WebRequester { get; set; }

        public async Task<Summoner> FindSummonerAsync(string region, string summonerName)
        {
            var uri = SummonerBaseUri + "by-name/" + Uri.EscapeDataString(summonerName);
            return (await WebRequester.SendRequestAsync(region, uri)).ToObject<Summoner>();
        }

        public async Task<ICollection<League>> GetLeaguesAsync(string region, int summonerId)
        {
            var uri = LeagueBaseUri + "by-summoner/" + summonerId + "/entry";
            var result = await WebRequester.SendRequestAsync(region, uri);
            return result[summonerId.ToString()].ToObject<ICollection<League>>();
        }

        public async Task<ICollection<RunePage>> GetRunePagesAsync(string region, int summonerId)
        {
            var uri = SummonerBaseUri + summonerId + "/runes";
            var result = await WebRequester.SendRequestAsync(region, uri);
            return result[summonerId.ToString()].ToObject<ICollection<RunePage>>();
        }
    }
}