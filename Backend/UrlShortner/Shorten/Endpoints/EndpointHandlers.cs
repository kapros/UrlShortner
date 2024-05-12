using UrlShortner.Domain;

namespace UrlShortner.Shorten.Endpoints;

public static class EndpointHandlers
{
    public static Func<ShortenUrlRequest, ShortUrlCommandHandler, HttpContext, ILogger, Task<IResult>> CreateShortLink()
    {
        return async (
request,
handler,
httpContext, logger) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("The specified URL is invalid.");
            }

            var shortenedUrl = await handler.Handle(new CreateShortUrlCommand(request.Url, httpContext.Request));

            return Results.Ok(new { ShortUrl = shortenedUrl });
        };
    }

    public static Func<Code, GetShortenedUrlQueryHandler, ILogger, Task<IResult>> GetByCode()
    {
        return async (code,
handler, logger) =>
        {
            var shortenedUrl = await handler.Handle(new GetShortenedUrlQuery(code));

            if (string.IsNullOrWhiteSpace(shortenedUrl))
            {
                return Results.NotFound();
            }

            return Results.Redirect(shortenedUrl);
        };
    }

    public static Func<GetShortenedUrlsQueryHandler, ILogger, Task<IResult>> GetAll()
    {
        return async (handler, logger) =>
        {
            var allUrls = await handler.Handle();

            return Results.Ok(allUrls);
        };
    }

    public static Func<Code, IUrlShorteningService, ILogger, Task<IResult>> DeleteByCode()
    {
        return async (code,
urlShorteningService, logger) =>
        {
            var record = await urlShorteningService.GetUrlFromCode(code);
            if (string.IsNullOrWhiteSpace(record))
            {
                return Results.NotFound();
            }

            await urlShorteningService.DeleteShortUrl(code);

            return Results.NoContent();
        };
    }

}
