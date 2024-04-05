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

    public static void RegisteUrlrServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRandomizer, Randomizer>();
        builder.Services.AddScoped<UrlShorteningService>();
        builder.Services.AddScoped<IUrlShorteningService>(
            x =>
            new CachedUrlShorteningService(x.GetRequiredService<UrlShorteningService>(), x.GetRequiredService<IMemoryCache>()));
    }

}
