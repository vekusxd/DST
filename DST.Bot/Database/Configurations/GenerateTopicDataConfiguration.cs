using DST.Bot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DST.Bot.Database.Configurations;

public class GenerateTopicDataConfiguration : IEntityTypeConfiguration<GenerateTopicData>
{
    public void Configure(EntityTypeBuilder<GenerateTopicData> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Country).HasMaxLength(255);
        builder.Property(t => t.Language).HasMaxLength(255);
        builder.Property(t => t.Scope).HasMaxLength(255);
        builder.Property(t => t.TimePeriod).HasMaxLength(255);

        builder.HasOne(t => t.User)
            .WithOne(u => u.GenerateTopicData)
            .HasForeignKey<GenerateTopicData>(u => u.UserId);
    }
}