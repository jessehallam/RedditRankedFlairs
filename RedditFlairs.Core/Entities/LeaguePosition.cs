using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace RedditFlairs.Core.Entities
{
    public class LeaguePosition
    {
        public int Id { get; set; }
        public string QueueType { get; set; }
        public string Rank { get; set; }
        [JsonIgnore]
        public virtual Summoner Summoner { get; set; }
        public string Tier { get; set; }

        public class EntityConfiguration : IEntityTypeConfiguration<LeaguePosition>
        {
            public void Configure(EntityTypeBuilder<LeaguePosition> builder)
            {
                builder.Property(x => x.QueueType).HasMaxLength(20).IsUnicode(false).IsRequired();
                builder.Property(x => x.Rank).HasMaxLength(20).IsUnicode(false).IsRequired();
                builder.Property(x => x.Tier).HasMaxLength(20).IsUnicode(false).IsRequired();
                builder.HasOne(x => x.Summoner).WithMany(x => x.LeaguePositions).IsRequired();
            }
        }
    }
}