using DST.Bot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DST.Bot.Database.Configurations;

public class FrontPageDataConfiguration : IEntityTypeConfiguration<FrontPageData>
{
    public void Configure(EntityTypeBuilder<FrontPageData> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Course).HasMaxLength(255);
        builder.Property(u => u.Group).HasMaxLength(255);
        builder.Property(u => u.Theme).HasMaxLength(255);
        builder.Property(u => u.Initials).HasMaxLength(255);
        builder.Property(u => u.Profile).HasMaxLength(255);
        builder.Property(u => u.SupervisorInitials).HasMaxLength(255);
        builder.Property(u => u.SupervisorAcademicDegree).HasMaxLength(255);
        builder.Property(u => u.SupervisorAcademicTitle).HasMaxLength(255);
        builder.Property(u => u.SupervisorJobTitle).HasMaxLength(255);

        builder.HasOne(d => d.User)
            .WithOne(u => u.FrontPageData)
            .HasForeignKey<FrontPageData>(u => u.UserId);

    }
}