using UrlShortner.Domain;

namespace UrlShortner.Shorten;

public interface IUrlShorteningService
{
    Task DeleteShortUrl(Code code);
    Task<Code> GenerateUniqueCode();
    Task<string?> GetUrlFromCode(Code code);
    Task<ShortUrl> ShortenUrl(string urlToShorten, HttpRequest httpRequest);
    Task<AllShortUrlsResponse> GetAllUrls();
}
