using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Services
{
    public class FlairService : IFlairService
    {
        private readonly IUnitOfWork _context;

        public FlairService(IUnitOfWork context)
        {
            _context = context;
        }

        public async Task<ICollection<User>> GetUsersForUpdateAsync(int max)
        {
            var users = _context.Users;
            var results = from user in users
                          where user.FlairUpdateRequiredTime.HasValue
                          orderby user.FlairUpdateRequiredTime ascending
                          select user;

            var result = await results.Take(max).ToListAsync();
            return result;
        }

        public async Task<bool> SetUpdateFlagAsync(IEnumerable<User> users, bool requiresUpdate = true)
        {
            foreach (var user in users)
            {
                user.FlairUpdateRequiredTime = requiresUpdate ? DateTimeOffset.Now : (DateTimeOffset?)null;
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetUpdatedAsync(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                user.FlairUpdateRequiredTime = null;
                user.FlairUpdatedTime = DateTimeOffset.Now;
            }
            return await _context.SaveChangesAsync() > 0;
        } 
    }
}