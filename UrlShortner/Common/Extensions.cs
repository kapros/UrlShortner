using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using UrlShortner.DataAccess;
using UrlShortner.Shorten;

namespace UrlShortner.Common;

public static class Extensions
{
    public static WebApplicationBuilder RegisterDevDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<UrlShortenerDbContext>(opt => opt.UseInMemoryDatabase("UrlShortener"));
        // can also seed the DB later
        builder.Services.AddMemoryCache();
        return builder;
    }

    public static WebApplicationBuilder RegisterUrlServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRandomizer, Randomizer>();
        builder.Services.AddScoped<UrlShorteningService>();
        builder.Services.AddScoped<IUrlShorteningService>(
            x =>
            new CachedUrlShorteningService(x.GetRequiredService<UrlShorteningService>(), x.GetRequiredService<IMemoryCache>()));
        return builder;
    }

    /// <summary>
    /// Adds other non domain specific services like:
    /// - response compression
    /// </summary>
    public static WebApplicationBuilder AddNonDomainServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(opts =>
        {
            opts.EnableForHttps = true;
            opts.Providers.Add<BrotliCompressionProvider>();
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes;
        });
        builder.Services.Configure<BrotliCompressionProviderOptions>(opts =>
        {
            opts.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Host.UseSerilog(logger);
        return builder;
    }

    public static WebApplicationBuilder RegisterHandlers(this WebApplicationBuilder builder)
    {
        builder.RegisterCommandHandlers();
        builder.RegisterQueryHandlers();
        return builder;
    }

    public static WebApplicationBuilder RegisterCommandHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ShortUrlCommandHandler>();
        return builder;
    }

    public static WebApplicationBuilder RegisterQueryHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<GetShortenedUrlQueryHandler>();
        return builder;
    }
}
