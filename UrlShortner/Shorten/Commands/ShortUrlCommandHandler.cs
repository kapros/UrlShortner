namespace UrlShortner.Shorten.Commands;

public class ShortUrlCommandHandler(IUrlShorteningService urlShorteningService) : ICommandHandler<CreateShortUrlCommand, string>
{
    // also thinking here that maybe the shortening itself could / should be broken down to generating the ID and the DB insert
    // in which case, technically, the command here is simplified to have no return but also be more atomic
    // on the other hand, it risks introducing unnecessary complexity in the calling handler
    public async Task<string> Handle(CreateShortUrlCommand input)
    {
        var result = await urlShorteningService.ShortenUrl(input.Url, input.HttpRequest);
        return result.Short;
    }
}
