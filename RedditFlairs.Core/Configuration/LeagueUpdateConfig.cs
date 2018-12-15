using System;

namespace RedditFlairs.Core.Configuration
{
    public class LeagueUpdateConfig
    {
        public bool Enable { get; set; }
        public TimeSpan ExecutionInterval { get; set; }
        public int MaximumCandidates { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }
}