// based on https://www.milanjovanovic.tech/blog/how-to-build-a-url-shortener-with-dotnet

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

builder.RegisteUrlServices();

builder.Services.AddCors();
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.MapPost("shorten", EndpointHandlers.CreateShortLink())
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
