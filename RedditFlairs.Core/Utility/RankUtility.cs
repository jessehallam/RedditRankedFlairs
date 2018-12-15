using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Core.Utility
{
    public class RankUtility
    {
        private readonly IDictionary<string, RankWeight> ranks;
        private readonly IDictionary<string, TierWeight> tiers;

        public RankUtility(FlairDbContext context)
        {
            ranks = context.RankWeights.ToList().ToDictionary(x => x.RankName, StringComparer.OrdinalIgnoreCase);
            tiers = context.TierWeights.ToList().ToDictionary(x => x.TierName, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<LeaguePosition> Filter(IEnumerable<LeaguePosition> positions, ICollection<string> queueTypes)
        {
            return positions.Where(position =>
            {
                return queueTypes.Any(queueType => queueType.Equals(position.QueueType, StringComparison.InvariantCultureIgnoreCase));
            });
        }

        public string Format(LeaguePosition position, string pattern)
        {
            var rules = FormatRuleDefinitions.Rules.ToDictionary(x => x.Tag);

            return Regex.Replace(pattern, @"%(\w)", match =>
            {
                var tagName = match.Groups[1].Value;
                if (!rules.TryGetValue(tagName, out var rule)) return "";
                return rule.Format(position);
            });

            //var paramDict = new Dictionary<string, string>
            //{
            //    {"t", position == null ? "unranked" : position.Tier.ToLowerInvariant()},
            //    {"T", position == null ? "Unranked" : Capitalize(position.Tier)},
            //    {"r", position == null ? "" : position.Rank.ToLowerInvariant()},
            //    {"R", position == null ? "" : position.Rank.ToUpperInvariant()}
            //};

            //return Regex.Replace(pattern, @"%(\w)", match => paramDict.ContainsKey(match.Groups[1].Value)
            //    ? match.Groups[1].Value
            //    : "").Trim();
        }

        public LeaguePosition GetBestPosition(IEnumerable<LeaguePosition> positions)
        {
            var query = from pos in positions
                select new {pos, weight = GetPositionOrdinal(pos)};

            return query.OrderByDescending(x => x.weight).FirstOrDefault()?.pos;
        }

        public int GetPositionOrdinal(LeaguePosition position)
        {
            if (!ranks.TryGetValue(position.Rank, out var rank))
                throw new InvalidOperationException("Unrecognized rank: " + position.Rank);

            if (!tiers.TryGetValue(position.Tier, out var tier))
                throw new InvalidOperationException("Unrecognized tier: " + position.Tier);

            return tier.Weight * 100 + rank.Weight;
        }

        private static string Capitalize(string s)
        {
            return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        }
    }
}