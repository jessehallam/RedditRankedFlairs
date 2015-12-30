using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Reddit;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Jobs
{
    public class BulkFlairUpdateJob
    {
        private const int MaxUsersPerLoop = 100;
        private static readonly TimeSpan NoUsersWaitInterval = TimeSpan.FromSeconds(30);
        private static readonly Mutex Lock = new Mutex();

        private readonly IFlairService _flairs;
        private readonly IUserService _users;
        private readonly IRedditService _reddit;
        private readonly ISubRedditService _subReddits;

        public BulkFlairUpdateJob(IUserService users, IFlairService flairs, IRedditService reddit, ISubRedditService subReddits)
        {
            _flairs = flairs;
            _users = users;
            _reddit = reddit;
            _subReddits = subReddits;
        }

        public void Execute()
        {
            if (!Lock.WaitOne(1000))
            {
                return;
            }
            try
            {
                ExecuteInternal().Wait();
            }
            finally
            {
                Lock.ReleaseMutex();
            }
        }

        private async Task ExecuteInternal()
        {
            while (true)
            {
                var users = await _flairs.GetUsersForUpdateAsync(MaxUsersPerLoop);
                var subReddits = await _subReddits.GetAllAsync();

                if (!users.Any())
                {
                    await Task.Delay(NoUsersWaitInterval);
                    continue;
                }

                var flairParams = (from u in users
                                   select new UserFlairParameter
                                   {
                                       Name = u.Name,
                                       Text = GetFlairText(u)
                                   }).ToList();

                foreach (var sub in subReddits)
                {
                    if (!await _reddit.SetUserFlairsAsync(sub.Name, flairParams))
                    {
                        throw new InvalidOperationException($"Update flair failed on /r/{sub.Name}.");
                    }
                }

                if (!await _flairs.SetUpdatedAsync(users))
                {
                    throw new InvalidOperationException("Unable to clear flair update flag.");
                }

                await Task.Delay(1);
            }
        }

        private static string GetFlairText(User user)
        {
            var summoner = user.ActiveSummoner;
            return summoner == null ? "" : LeagueUtil.Stringify(summoner.LeagueInfo);
        }
    }
}