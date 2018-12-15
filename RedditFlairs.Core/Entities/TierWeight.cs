using System.ComponentModel.DataAnnotations;

namespace RedditFlairs.Core.Entities
{
    public class TierWeight
    {
        [Key]
        public string TierName { get; set; }
        public int Weight { get; set; }
    }
}