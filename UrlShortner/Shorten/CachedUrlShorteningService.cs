﻿
using Microsoft.Extensions.Caching.Memory;
using UrlShortner.DataAccess;

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
        _memoryCache.Set(shortUrl.Code, shortUrl);
        return shortUrl;
    }

    public Task<Code> GenerateUniqueCode() => _decorated.GenerateUniqueCode();

    public Task<string?> GetUrlFromCode(Code code)
    {
        return _memoryCache.GetOrCreateAsync(code, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return _decorated.GetUrlFromCode(code);
        });
    }
}
