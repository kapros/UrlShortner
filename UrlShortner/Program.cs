// based on https://www.milanjovanovic.tech/blog/how-to-build-a-url-shortener-with-dotnet
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (builder.Environment.IsDevelopment())
{
    builder.RegisterDevDependencies();
}

builder.RegisterUrlServices();

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

app.MapGet("{code}", EndpointHandlers.GetByCode())
.WithName("get")
.WithOpenApi();

app.MapDelete("{code}", async (Code code,
            IUrlShorteningService urlShorteningService) =>
{
    if (string.IsNullOrWhiteSpace((await urlShorteningService.GetUrlFromCode(code))))
    {
        return Results.NotFound();
    }

    await urlShorteningService.DeleteShortUrl(code);

    return Results.NoContent();
})
.WithName("delete")
.WithOpenApi();

app.Run();


namespace UrlShortner
{
    public partial class Program
    {
        // Expose the Program class for use with WebApplicationFactory<T>
    }
}