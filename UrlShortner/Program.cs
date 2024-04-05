using Microsoft.Extensions.Caching.Memory;
using UrlShortner.Common;
using UrlShortner.DataAccess;
using UrlShortner.Shorten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (builder.Environment.IsDevelopment())
{
    builder.RegisterDevDependencies();
}

builder.RegisteUrlrServices();

builder.Services.AddCors();
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.MapPost("shorten", async (
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

    return Results.Ok(new { ShortUrl = shortenedUrl.Short});
})
.WithName("create")
.WithOpenApi();

app.MapGet("{code}", async (Code code,
    IUrlShorteningService urlShorteningService) =>
{
    var shortenedUrl = await urlShorteningService.GetUrlFromCode(code);

    if (shortenedUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(shortenedUrl);
})
.WithName("get")
.WithOpenApi();

app.Run();
