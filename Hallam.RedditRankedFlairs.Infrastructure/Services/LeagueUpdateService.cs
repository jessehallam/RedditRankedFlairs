using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Services
{
    public class LeagueUpdateService : ILeagueUpdateService
    {
        private static readonly TimeSpan LeagueStaleTime = TimeSpan.FromHours(4);

        private readonly IUnitOfWork _context;

        public LeagueUpdateService(IUnitOfWork context)
        {
            _context = context;
        }

        public async Task<ICollection<Summoner>> GetSummonersForUpdateAsync(int max)
        {
            var cutoff = DateTimeOffset.Now - LeagueStaleTime;
            var query = from summoner in _context.Summoners
                        where summoner.LeagueInfo.UpdatedTime.HasValue
                              && summoner.LeagueInfo.UpdatedTime < cutoff
                        select summoner;
            return await query.Take(max).ToListAsync();
        }

        public async Task<bool> SetUpdatedAsync(IEnumerable<Summoner> summoners)
        {
            foreach (var s in summoners)
            {
                s.LeagueInfo.UpdatedTime = DateTimeOffset.Now;
            }
            return await _context.SaveChangesAsync() > 0;
        } 
    }
}