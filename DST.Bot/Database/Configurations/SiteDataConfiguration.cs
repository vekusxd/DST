using DST.Bot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DST.Bot.Database.Configurations;

public class SiteDataConfiguration : IEntityTypeConfiguration<SiteData>
{
    public void Configure(EntityTypeBuilder<SiteData> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).HasMaxLength(255);
        builder.Property(s => s.Url).HasMaxLength(255);
        
        builder.HasOne(s => s.User)
            .WithOne(u => u.SiteData)
            .HasForeignKey<SiteData>(u => u.UserId);
    }
}