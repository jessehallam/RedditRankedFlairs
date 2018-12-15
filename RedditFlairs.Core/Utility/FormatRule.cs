using System;
using System.Collections.Generic;
using System.Linq;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Core.Utility
{
    public class FormatRule
    {
        public Func<LeaguePosition, string> Format { get; set; }
        public string Tag { get; set; }
    }

    public static class FormatRuleDefinitions
    {
        public static readonly List<FormatRule> Rules = new List<FormatRule>
        {
            new FormatRule
            {
                Format = (position) => position == null ? "unranked" : position.Tier.ToLowerInvariant(),
                Tag = "t"
            },
            new FormatRule
            {
                Format = (position) => position == null ? "Unranked" : Capitalize(position.Tier),
                Tag = "T"
            },
            new FormatRule
            {
                Format = (position) => position == null ? "" : position.Rank.ToLowerInvariant(),
                Tag = "r"
            },
            new FormatRule
            {
                Format = (position) => position == null ? "" : position.Rank.ToUpperInvariant(),
                Tag = "R"
            }
        };

        private static string Capitalize(string s)
        {
            if (s.Length == 0) return s;
            return char.ToUpperInvariant(s.First()) + s.Substring(1).ToLowerInvariant();
        }
    }
}