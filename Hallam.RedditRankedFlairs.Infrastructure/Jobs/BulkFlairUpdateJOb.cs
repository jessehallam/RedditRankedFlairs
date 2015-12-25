using System;
using System.Threading;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Jobs
{
    public class BulkFlairUpdateJob
    {
        private static readonly TimeSpan MaxRunTime = TimeSpan.FromMinutes(10);
        private static readonly Mutex Lock = new Mutex(); 
        private readonly IUserService _users;

        public BulkFlairUpdateJob(IUserService users)
        {
            _users = users;
        }

        public void Execute()
        {
            if (!Lock.WaitOne(1000))
            {
                return;
            }
            try
            {
                // ExecuteInternal().Wait();
            }
            finally
            {
                Lock.ReleaseMutex();
            }
        }
    }
}