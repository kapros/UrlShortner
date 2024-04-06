using UrlShortner.DataAccess;

namespace UrlShortner.Shorten;

public static class EndpointHandlers
{
    public static Func<ShortenUrlRequest, IUrlShorteningService, UrlShortnerDbContext, HttpContext, Task<IResult>> CreateShortLink()
    {
        return async (
            ShortenUrlRequest request,
            IUrlShorteningService urlShorteningService,
            UrlShortnerDbContext dbContext,
            HttpContext httpContext) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("The specified URL is invalid.");
            }

            var code = await urlShorteningService.GenerateUniqueCode();

            var httpRequest = httpContext.Request;

            var shortenedUrl = new ShortUrl
            {
                Id = Guid.NewGuid(),
                Long = request.Url,
                Code = code,
                Short = $"{httpRequest.Scheme}://{httpRequest.Host}/{code}",
                CreatedOnUtc = DateTime.UtcNow
            };
            // TODO: move to service
            dbContext.ShortenedUrls.Add(shortenedUrl);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new { ShortUrl = shortenedUrl.Short });
        };
    }


}
