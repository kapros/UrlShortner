using Microsoft.Extensions.Caching.Memory;
using UrlShortner.DataAccess;
using UrlShortner.Shorten;

namespace UrlShortner.Common;

public static class Extensions
{
    public static void RegisterDevDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<UrlShortnerDbContext>(opt => opt.UseInMemoryDatabase("UrlShortener"));
        // can also seed the DB later
        builder.Services.AddMemoryCache();
    }

    public static void RegisterUrlServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRandomizer, Randomizer>();
        builder.Services.AddScoped<UrlShorteningService>();
        builder.Services.AddScoped<IUrlShorteningService>(
            x =>
            new CachedUrlShorteningService(x.GetRequiredService<UrlShorteningService>(), x.GetRequiredService<IMemoryCache>()));
    }


    public static void RegisterHandlers(this WebApplicationBuilder builder)
    {
        builder.RegisterCommandHandlers();
        builder.RegisterQueryHandlers();
    }

    public static void RegisterCommandHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ShortUrlCommandHandler>();
    }

    public static void RegisterQueryHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<GetShortenedUrlQueryHandler>();
    }
}
