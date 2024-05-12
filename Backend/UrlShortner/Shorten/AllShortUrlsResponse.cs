namespace UrlShortner.Shorten;

public record AllShortUrlsResponse(IEnumerable<ShortUrlResponse> shortUrls);
