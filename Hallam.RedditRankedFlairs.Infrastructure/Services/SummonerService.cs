using System.Data.Entity;
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
    }
}