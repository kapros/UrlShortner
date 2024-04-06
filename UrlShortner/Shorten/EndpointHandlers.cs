using UrlShortner.DataAccess;

namespace UrlShortner.Shorten;

public static class EndpointHandlers
{
    public static Func<ShortenUrlRequest, IUrlShorteningService, HttpContext, Task<IResult>> CreateShortLink()
    {
        return async (
            ShortenUrlRequest request,
            IUrlShorteningService urlShorteningService,
            HttpContext httpContext) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("The specified URL is invalid.");
            }

            var shortenedUrl = await urlShorteningService.ShortenUrl(request.Url, httpContext.Request);


            return Results.Ok(new { ShortUrl = shortenedUrl.Short });
        };
    }

    public static Func<Code, IUrlShorteningService, Task<IResult>> GetByCode()
    {
        return async (Code code,
            IUrlShorteningService urlShorteningService) =>
        {
            var shortenedUrl = await urlShorteningService.GetUrlFromCode(code);

            if (string.IsNullOrWhiteSpace(shortenedUrl))
            {
                return Results.NotFound();
            }

            return Results.Redirect(shortenedUrl);
        };
    }

    public static Func<Code, IUrlShorteningService, Task<IResult>> DeleteByCode()
{
        return async (Code code,
                    IUrlShorteningService urlShorteningService) =>
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
