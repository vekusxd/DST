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

        builder.Property(u => u.DialogStateId)
            .HasDefaultValue(DialogStateId.DefaultState);

        builder.HasOne(u => u.FrontPageData)
            .WithOne(d => d.User)
            .HasForeignKey<FrontPageData>(da => da.UserId);
        
        builder.Navigation(u => u.FrontPageData).AutoInclude();

    }
}