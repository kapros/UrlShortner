namespace UrlShortner.Shorten.Queries;

public class GetShortenedUrlsQueryHandler(IUrlShorteningService urlShorteningService) : IQueryHandler<AllShortUrlsResponse>
{
    public async Task<AllShortUrlsResponse> Handle() => await urlShorteningService.GetAllUrls();
}
