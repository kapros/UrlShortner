using System.Dynamic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using UrlShortner.DataAccess;

namespace UrlShortner.Tests;
public class UrlShorteningTests
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UrlShorteningTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        //var host = factory.Server.Host; 
        // TODO check if above line needs to be fixed
        // System.InvalidOperationException: 'The TestServer constructor was not called with a IWebHostBuilder so IWebHost is not available.'
        using var scope = _factory.Server.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<UrlShortenerDbContext>();
        dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task PostCreatesShortUrl()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("shorten", JsonContent.Create(new { url = "https://test.com" }));
        var content = await response.Content.ReadAsStringAsync() ?? "";
        string shortUrl = JsonConvert.DeserializeObject<dynamic>(content).shortUrl;
        Assert.NotEqual(shortUrl, "https://test.com");
        Assert.True(shortUrl.Contains( "localhost"));

        var memCache = _factory.Server.Services.GetRequiredService(typeof(IMemoryCache)) as IMemoryCache;
        Assert.NotNull(memCache);
        var cached = memCache.Get(shortUrl.Split("localhost/").Last());
        Assert.NotNull(cached);
    }
}

//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
