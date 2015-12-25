using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Services
{
    public class SubRedditService : ISubRedditService
    {
        private readonly IUnitOfWork _context;

        public SubRedditService(IUnitOfWork context)
        {
            _context = context;
        }

        public async Task<ICollection<SubReddit>> GetAllAsync()
        {
            return await _context.SubReddits.OrderBy(sub => sub.Name).ToListAsync();
        }
    }
}