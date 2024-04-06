using System.Dynamic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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

        using (var scope = factory.Server.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<UrlShortnerDbContext>();

            dbContext.Database.EnsureCreated();
        }
    }

    [Fact]
    public async Task PostCreatesShortUrl()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("shorten", JsonContent.Create(new { url = "https://test.com"}));

        var content = await response.Content.ReadAsStringAsync() ?? "";
        string shortUrl = JsonConvert.DeserializeObject<dynamic>(content).shortUrl;
        Assert.NotEqual(shortUrl, "https://test.com");
        Assert.True(shortUrl.Contains( "localhost"));
    }
}
