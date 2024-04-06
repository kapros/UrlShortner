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
public class CreatingShortUrlsTests
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CreatingShortUrlsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        using var scope = _factory.Server.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<UrlShortnerDbContext>();
        dbContext.Database.EnsureCreated();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PostCreatesShortUrl()
    {
        var response = await _client.PostAsync("shorten", JsonContent.Create(new { url = "https://test.com" }));
        var content = await response.Content.ReadAsStringAsync();
        string shortUrl = JsonConvert.DeserializeObject<dynamic>(content).shortUrl;
        Assert.NotEqual(shortUrl, "https://test.com");
        Assert.True(shortUrl.Contains("localhost"));
    }

    [Fact]
    public async Task PostCreatesCacheEntry()
    {
        var response = await _client.PostAsync("shorten", JsonContent.Create(new { url = "https://test.com" }));
        var content = await response.Content.ReadAsStringAsync();
        string shortUrl = JsonConvert.DeserializeObject<dynamic>(content).shortUrl;
        var code = shortUrl.Split("localhost/").Last();

        var memCache = _factory.Server.Services.GetRequiredService(typeof(IMemoryCache)) as IMemoryCache;
        Assert.NotNull(memCache);
        var cached = memCache.Get(code);
        Assert.NotNull(cached);
    }

    [Fact]
    public async Task PostCreatesDbEntry()
    {
        var response = await _client.PostAsync("shorten", JsonContent.Create(new { url = "https://test.com" }));
        var content = await response.Content.ReadAsStringAsync();
        string shortUrl = JsonConvert.DeserializeObject<dynamic>(content).shortUrl;
        var code = shortUrl.Split("localhost/").Last();

        using var scope = _factory.Server.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<UrlShortnerDbContext>();
        var allEntries = await dbContext.ShortenedUrls.ToListAsync();
        Assert.Equal(1, allEntries.Count);
        Assert.Equal("https://test.com", allEntries.First().Long);
    }
}
