using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RedditFlairs.Core.Entities
{
    public class User
    {
        public virtual ICollection<UserFlair> Flairs { get; set; }
        public DateTimeOffset? FlairsUpdated { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Summoner> Summoners { get; set; }

        public class EntityConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.Property(x => x.Name)
                    .HasMaxLength(40).IsUnicode(false).IsRequired();

                builder.HasAlternateKey(x => x.Name);
            }
        }
    }
}