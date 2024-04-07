namespace UrlShortner.Shorten.Queries;

public class GetShortenedUrlQueryHandler(IUrlShorteningService urlShorteningService) : IQueryHandler<GetShortenedUrlQuery, string?>
{
    public async Task<string?> Handle(GetShortenedUrlQuery query) => await urlShorteningService.GetUrlFromCode(query.Code);
}
