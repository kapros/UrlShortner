using UrlShortner.Settings;

namespace UrlShortner.DataAccess;

public class UrlShortnerDbContext : DbContext
{
    public UrlShortnerDbContext(DbContextOptions options)
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
                .HasConversion(new CodeConverter());

            builder.Property(shortUrl => shortUrl.Short).HasColumnName("ShortUrl");

            builder.Property(shortUrl => shortUrl.Long).HasColumnName("LongUrl");

            builder
                .HasIndex(shortUrl => shortUrl.Code)
                .IsUnique();
        });
    }
}
