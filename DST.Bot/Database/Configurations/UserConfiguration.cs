using DST.Bot.Entities;
using DST.Bot.Features.StateManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DST.Bot.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.ChatId);

        builder.Property(u => u.ChatId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(u => u.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.ArticleSearchTerm)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.PsychologicalTestPoints)
            .HasDefaultValue(0);

        builder.HasOne(u => u.FrontPageData)
            .WithOne(d => d.User)
            .HasForeignKey<FrontPageData>(da => da.UserId);

        builder.HasOne(u => u.GenerateTopicData)
            .WithOne(t => t.User)
            .HasForeignKey<GenerateTopicData>(t => t.UserId);

        builder.Property(u => u.PsychologicalType)
            .HasDefaultValue(PsychologicalType.NotSet);

        builder.Navigation(u => u.FrontPageData).AutoInclude();
        builder.Navigation(u => u.GenerateTopicData).AutoInclude();
    }
}

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