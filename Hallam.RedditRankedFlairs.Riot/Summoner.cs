﻿namespace Hallam.RedditRankedFlairs.Riot
{
    public class Summoner
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int ProfileIconId { get; set; }
        public long RevisionDate { get; set; }
        public int SummonerLevel { get; set; }
    }
}