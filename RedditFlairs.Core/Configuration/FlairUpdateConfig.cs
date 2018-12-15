using System;

namespace RedditFlairs.Core.Configuration
{
    public class FlairUpdateConfig
    {
        public bool Enable { get; set; }
        public TimeSpan ExecutionInterval { get; set; }
        public int MaximumCandidates { get; set; }
    }
}