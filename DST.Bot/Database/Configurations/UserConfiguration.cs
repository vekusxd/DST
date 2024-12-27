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

        builder.Property(u => u.DialogState)
            .HasMaxLength(255)
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

        builder.HasOne(u => u.BookDesignData)
            .WithOne(b => b.User)
            .HasForeignKey<BookDesignData>(b => b.UserId);
    }
}