using Microsoft.AspNetCore.TestHost;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    [Fact]
    public async Task ShouldDisplayAllLinks()
    {
        var pw = await Playwright.CreateAsync();
        var browser = pw["Chromium"];
        var launchOptions = new BrowserTypeLaunchOptions();
        launchOptions.Headless = false;
        var context = await browser.LaunchAsync(launchOptions);
        var page = await context.NewPageAsync();
        var frontendUrl = _appHost.GetEndpoint("frontend");
        await page.GotoAsync(frontendUrl.AbsoluteUri);
        await page.GetByText("New").ClickAsync();
        var linkToShorten = "https://github.com/dotnet/aspire";
        await page.Locator("[data-test-id='links-input']").FillAsync(linkToShorten);
        await page.Locator("button[type='submit']").ClickAsync();
        var shortLink = await page.Locator("[data-test-id='short-link']").TextContentAsync();
        await page.GetByText("All links").ClickAsync();
        await page.GetByText(linkToShorten).WaitForAsync();
        await page.GetByText(shortLink).WaitForAsync();


    }
}