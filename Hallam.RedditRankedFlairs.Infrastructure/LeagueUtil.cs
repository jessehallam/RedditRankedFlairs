﻿using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs
{
    public class LeagueUtil
    {
        private static readonly string[] DivisionNames = new[]
        {
            "I", "II", "III", "IV", "V"
        };

        public static string Stringify(LeagueInfo league)
        {
            if (league == null) return "";
            if (league.UpdatedTime.HasValue == false || league.Division == 0) return "";
            return league.Tier.ToString() + " " + DivisionNames[league.Division - 1];
        } 
    }
}