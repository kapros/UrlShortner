using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace UrlShortner.Tests;
public class UrlShorteningTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UrlShorteningTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostCreatesShortUrl()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/shorten", JsonContent.Create(new { url = "https://test.com"}));

        // Assert
        response.EnsureSuccessStatusCode(); 
    }
}

//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
