
using Microsoft.Extensions.Caching.Memory;
using UrlShortner.Domain;

namespace UrlShortner.Shorten;

public class CachedUrlShorteningService : IUrlShorteningService
{
    private readonly IUrlShorteningService _decorated;
    private readonly IMemoryCache _memoryCache;

    public CachedUrlShorteningService(IUrlShorteningService urlShorteningService, IMemoryCache memoryCache)
    {
        _decorated = urlShorteningService;
        _memoryCache = memoryCache;
    }

    public async Task<ShortUrl> ShortenUrl(string urlToShorten, HttpRequest httpRequest) 
    { 
        var shortUrl = await _decorated.ShortenUrl(urlToShorten, httpRequest);
        _memoryCache.Set(shortUrl.Code.code, shortUrl.Long);
        return shortUrl;
    }

    public Task<Code> GenerateUniqueCode() => _decorated.GenerateUniqueCode();

    public Task<string?> GetUrlFromCode(Code code)
    {
        return _memoryCache.GetOrCreateAsync(code.code, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return _decorated.GetUrlFromCode(code);
        });
    }

    public async Task DeleteShortUrl(Code code)
    {
        try
        {
            await _decorated.DeleteShortUrl(code);
            _memoryCache.Remove(code);
        }
        catch { }
    }
}
