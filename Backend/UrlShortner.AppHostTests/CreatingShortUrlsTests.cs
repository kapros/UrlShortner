using Newtonsoft.Json;
using System.Net.Http.Json;

namespace UrlShortner.AppHostTests;

public class CreatingShortUrlsTests : IClassFixture<AspireTestBase>, IDisposable
{
    private readonly DistributedApplication _appHost;
    private readonly HttpClient _client;

    public CreatingShortUrlsTests(AspireTestBase test)
    {
        _appHost = test.App;

        _client = _appHost.CreateHttpClient("shortener");
        _client.BaseAddress = new Uri(_client.BaseAddress.AbsoluteUri + "api/v1/");
    }

    public void Dispose()
    {
        // get services here and clean the DB if needed to tear down each test
    }

    [Fact]
    public async Task PostShouldCreateShortLink()
    {
        var response = await _client.PostAsync("shorten", JsonContent.Create(new { url = "https://test.com" }));
        var content = await response.Content.ReadAsStringAsync();
        string shortUrl = JsonConvert.DeserializeObject<dynamic>(content).shortUrl;
        Assert.NotEqual("https://test.com", shortUrl);
        Assert.Contains("localhost", shortUrl);
    }
}