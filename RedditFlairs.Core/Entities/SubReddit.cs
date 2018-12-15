using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RedditFlairs.Core.Entities
{
    public class SubReddit
    {
        public string CssPattern { get; set; }
        public string FlairPattern { get; set; }

        [Key]
        public string Name { get; set; }

        public string QueueTypes { get; set; }

        public class EntityConfiguration : IEntityTypeConfiguration<SubReddit>
        {
            public void Configure(EntityTypeBuilder<SubReddit> builder)
            {
                builder.Property(x => x.CssPattern)
                    .HasMaxLength(20).IsUnicode(false).HasDefaultValue("").IsRequired();

                builder.Property(x => x.FlairPattern)
                    .HasMaxLength(20).IsUnicode(false).HasDefaultValue("").IsRequired();

                builder.Property(x => x.Name)
                    .HasMaxLength(20).IsUnicode(false).IsRequired();

                builder.Property(x => x.QueueTypes)
                    .HasMaxLength(200).IsUnicode(false).HasDefaultValue("").IsRequired();
            }
        }
    }
}