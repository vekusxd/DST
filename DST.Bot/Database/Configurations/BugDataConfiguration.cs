using DST.Bot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DST.Bot.Database.Configurations;

public class BugDataConfiguration : IEntityTypeConfiguration<BugData>
{
    public void Configure(EntityTypeBuilder<BugData> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).HasMaxLength(255);
        
        builder.Property(s => s.Content).HasMaxLength(-1);

        builder.HasOne(b => b.User)
            .WithOne(u => u.BugData)
            .HasForeignKey<BugData>(u => u.UserId);
    }
}