namespace RedditFlairs.Core.Configuration
{
    public class TasksConfigSection
    {
        public bool Enable { get; set; }
        public FlairPushConfig FlairPush { get; set; }
        public FlairUpdateConfig FlairUpdate { get; set; }
        public LeagueUpdateConfig LeagueUpdate { get; set; }
        public ValidationConfig Validation { get; set; }
    }
}