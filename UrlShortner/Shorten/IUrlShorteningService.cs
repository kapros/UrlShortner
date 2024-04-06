namespace UrlShortner.Shorten;

public interface IUrlShorteningService
{
    Task<Code> GenerateUniqueCode();
    Task<string?> GetUrlFromCode(Code code);
    Task<ShortUrl> ShortenUrl(string urlToShorten, HttpRequest httpRequest);
}
