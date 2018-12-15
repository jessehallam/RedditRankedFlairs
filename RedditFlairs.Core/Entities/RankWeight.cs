using System.ComponentModel.DataAnnotations;

namespace RedditFlairs.Core.Entities
{
    public class RankWeight
    {
        [Key]
        public string RankName { get; set; }
        public int Weight { get; set; }
    }
}