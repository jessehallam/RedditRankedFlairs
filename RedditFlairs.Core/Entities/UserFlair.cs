using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace RedditFlairs.Core.Entities
{
    public class UserFlair
    {
        public string CssText { get; set; }
        public int Id { get; set; }
        public bool NeedToSend { get; set; }
        public string Text { get; set; }

        /// <summary>
        ///     The sub reddit that this flair applies to.
        /// </summary>
        public virtual SubReddit SubReddit { get; set; }

        /// <summary>
        ///     Indicates when the flair text was updated in the system.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public class EntityConfiguration : IEntityTypeConfiguration<UserFlair>
        {
            public void Configure(EntityTypeBuilder<UserFlair> builder)
            {
                builder.Property(x => x.CssText)
                    .HasMaxLength(50).IsUnicode(false).HasDefaultValue("").IsRequired();

                builder.Property(x => x.Text)
                    .HasMaxLength(50).IsUnicode(false).HasDefaultValue("").IsRequired();

                builder
                    .HasOne(x => x.User)
                    .WithMany(x => x.Flairs)
                    .IsRequired();
            }
        }
    }
}