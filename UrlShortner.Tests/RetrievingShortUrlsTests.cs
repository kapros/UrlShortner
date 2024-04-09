using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using UrlShortner.DataAccess;
using UrlShortner.Domain;

namespace UrlShortner.Tests;
public class RetrievingShortUrlsTests
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly char[] _code = "a1Bc12".ToCharArray();
    public RetrievingShortUrlsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        using var scope = _factory.Server.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<UrlShortenerDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ShortenedUrls.Add(new ShortUrl
        {
            Code = Code.Create(_code),
            CreatedOnUtc = DateTime.UtcNow,
            Short = Link.Create("https://localhost/" + new string(_code)),
            Long = Link.Create("https://test.com")
        });
        dbContext.SaveChanges();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task GetFetchesTheUrl()
    {
        var response = await _client.GetAsync(new string(_code));
        Assert.Equal(expected: HttpStatusCode.Found, response.StatusCode);
        var redirect = response.Headers.Location;
        Assert.Equal("https://test.com/", redirect.ToString());
    }

    [Fact]
    public async Task GetCachesTheResultIfNotPresent()
    {
        var response = await _client.GetAsync(new string(_code));
        var memCache = _factory.Server.Services.GetRequiredService(typeof(IMemoryCache)) as IMemoryCache;
        Assert.NotNull(memCache);
        var cached = memCache.Get(_code);
        Assert.NotNull(cached);
    }
}
