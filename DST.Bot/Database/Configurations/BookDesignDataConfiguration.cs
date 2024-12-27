using DST.Bot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DST.Bot.Database.Configurations;

public class BookDesignDataConfiguration : IEntityTypeConfiguration<BookDesignData>
{
    public void Configure(EntityTypeBuilder<BookDesignData> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Isbn).HasMaxLength(255);
        builder.Property(b => b.Publisher).HasMaxLength(255);
        builder.Property(b => b.AuthorInitials).HasMaxLength(255);
        builder.Property(b => b.AuthorSurname).HasMaxLength(255);
        builder.Property(b => b.BookTitle).HasMaxLength(255);
        builder.Property(b => b.PublicationDetails).HasMaxLength(255);
        builder.Property(b => b.PublicationPlace).HasMaxLength(255);

        builder.HasOne(b => b.User)
            .WithOne(u => u.BookDesignData)
            .HasForeignKey<BookDesignData>(u => u.UserId);
    }
}