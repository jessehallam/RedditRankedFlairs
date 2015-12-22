using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Services
{
    public class SummonerService : ISummonerService
    {
        protected IUnitOfWork UnitOfWork;

        public SummonerService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<Summoner> AddSummonerAsync(User user, int summonerId, string region, string name)
        {
            user.Summoners.Add(new Summoner
            {
                LeagueInfo = new LeagueInfo(),
                Name = name,
                Region = region,
                SummonerId = summonerId,
                User = user
            });
            await UnitOfWork.SaveChangesAsync();
            return user.Summoners.First(summoner => summoner.SummonerId == summonerId);
        } 

        public Task<Summoner> FindAsync(string region, string summonerName)
        {
            return UnitOfWork.Summoners.FirstOrDefaultAsync(summoner =>
                summoner.Region == region &&
                summoner.Name == summonerName);
        }

        public Task<bool> IsSummonerRegistered(string region, string summonerName)
        {
            return UnitOfWork.Summoners.AnyAsync(summoner =>
                summoner.Region == region &&
                summoner.Name == summonerName);
        }

        public async Task<bool> RemoveAsync(string region, string summonerName)
        {
            var entity = await FindAsync(region, summonerName);
            if (entity == null) return false;
            UnitOfWork.Summoners.Remove(entity);
            return await UnitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetActiveSummonerAsync(Summoner summoner)
        {
            summoner.User.ActiveSummoner = summoner;
            return await UnitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateLeagueAsync(Summoner summoner, TierName tier, int division)
        {
            summoner.LeagueInfo.Division = division;
            summoner.LeagueInfo.Tier = tier;
            summoner.LeagueInfo.UpdatedTime = DateTimeOffset.Now;
            return await UnitOfWork.SaveChangesAsync() > 0;
        } 
    }
}