using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RedditFlairs.Core.Entities
{
    public class SummonerValidation
    {
        public DateTimeOffset? AttemptedAt { get; set; }
        public int Attempts { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }
        public ValidationStatus Status { get; set; }

        public class EntityConfiguration : IEntityTypeConfiguration<SummonerValidation>
        {
            public void Configure(EntityTypeBuilder<SummonerValidation> builder)
            {
                builder.Property(x => x.Code)
                    .HasMaxLength(20).IsUnicode(false).IsRequired();
            }
        }
    }
}