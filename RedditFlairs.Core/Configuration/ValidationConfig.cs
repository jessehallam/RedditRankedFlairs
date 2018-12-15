using System;

namespace RedditFlairs.Core.Configuration
{
    public class ValidationConfig
    {
        public TimeSpan AttemptInterval { get; set; }
        public bool Enable { get; set; }
        public TimeSpan ExecutionInterval { get; set; }
        public int MaximumAttempts { get; set; }
        public int MaximumCandidates { get; set; }
    }
}