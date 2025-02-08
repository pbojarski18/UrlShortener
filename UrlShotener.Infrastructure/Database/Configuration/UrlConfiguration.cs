using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Database.Configuration;

public class UrlConfiguration : IEntityTypeConfiguration<UrlEntity>
{
    public void Configure(EntityTypeBuilder<UrlEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalUrl)
               .IsRequired();

        builder.Property(x => x.ShortUrl)
               .IsRequired();

        builder.Property(x => x.CreatedTime)
               .IsRequired()
               .HasPrecision(7);

        builder.Property(x => x.ExpirationDate)
                .IsRequired()
                .HasPrecision(7);

        builder.HasIndex(p => p.ShortUrl).IsUnique();
    }
}