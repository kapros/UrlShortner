using UrlShortner.Domain;

namespace UrlShortner.Shorten;

public static class EndpointHandlers
{
    public static Func<ShortenUrlRequest, ShortUrlCommandHandler, HttpContext, ILogger, Task<IResult>> CreateShortLink()
    {
        return async (
            ShortenUrlRequest request,
            ShortUrlCommandHandler handler,
            HttpContext httpContext, ILogger logger) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("The specified URL is invalid.");
            }

            var shortenedUrl = await handler.Handle(new CreateShortUrlCommand( request.Url, httpContext.Request));

            return Results.Ok(new { ShortUrl = shortenedUrl });
        };
    }

    public static Func<Code, GetShortenedUrlQueryHandler, ILogger, Task<IResult>> GetByCode()
    {
        return async (Code code,
            GetShortenedUrlQueryHandler handler, ILogger logger) =>
        {
            var shortenedUrl = await handler.Handle(new GetShortenedUrlQuery(code));

            if (string.IsNullOrWhiteSpace(shortenedUrl))
            {
                return Results.NotFound();
            }

            return Results.Redirect(shortenedUrl);
        };
    }

    public static Func<Code, IUrlShorteningService, ILogger, Task<IResult>> DeleteByCode()
{
        return async (Code code,
                    IUrlShorteningService urlShorteningService, ILogger logger) =>
        {
            if (string.IsNullOrWhiteSpace((await urlShorteningService.GetUrlFromCode(code))))
            {
                return Results.NotFound();
            }

            await urlShorteningService.DeleteShortUrl(code);

            return Results.NoContent();
        };
    }

}
