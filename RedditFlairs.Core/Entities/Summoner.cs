using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace RedditFlairs.Core.Entities
{
    public class Summoner
    {
        public string AccountId { get; set; }
        public int Id { get; set; }
        public virtual ICollection<LeaguePosition> LeaguePositions { get; set; }
        public string PUUID { get; set; }
        public DateTimeOffset? RankUpdatedAt { get; set; }
        public string Region { get; set; }
        public string SummonerId { get; set; }
        public string SummonerName { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        public virtual SummonerValidation Validation { get; set; }

        public class EntityConfiguration : IEntityTypeConfiguration<Summoner>
        {
            public void Configure(EntityTypeBuilder<Summoner> builder)
            {
                builder.Property(x => x.AccountId)
                    .HasMaxLength(100).IsUnicode(false).IsRequired();

                builder.Property(x => x.Region)
                    .HasMaxLength(8).IsUnicode(false).IsRequired();

                builder.Property(x => x.PUUID)
                    .HasMaxLength(100).IsUnicode(false).IsRequired();

                builder.Property(x => x.SummonerId)
                    .HasMaxLength(100).IsUnicode(false).IsRequired();

                builder.HasOne(x => x.User)
                    .WithMany(x => x.Summoners)
                    .IsRequired();

                builder
                    .HasOne(x => x.Validation)
                    .WithOne()
                    .HasForeignKey<Summoner>(x => x.Id)
                    .HasPrincipalKey<SummonerValidation>(x => x.Id);
            }
        }
    }
}