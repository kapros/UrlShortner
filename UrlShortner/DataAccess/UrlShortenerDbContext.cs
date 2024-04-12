﻿using UrlShortner.Domain;
using UrlShortner.Settings;

namespace UrlShortner.DataAccess;

public class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<ShortUrl> ShortenedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>(builder =>
        {
            builder
                .Property(shortUrl => shortUrl.Code)
                .HasMaxLength(ShortUrlSettings.Length)
                //.HasConversion<string>()
                .HasConversion(new CodeConverter())
                .HasMaxLength(10);

            builder.Property(shortUrl => shortUrl.Short).HasColumnName("ShortUrl");

            builder.Property(shortUrl => shortUrl.Long).HasColumnName("LongUrl");

            builder
                .HasIndex(shortUrl => shortUrl.Code)
                .IsUnique();
        });
    }
}