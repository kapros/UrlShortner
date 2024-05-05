using System.Dynamic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using UrlShortner.DataAccess;
using UrlShortner.Domain;
using UrlShortner.Jobs;

namespace UrlShortner.Tests;
public class DeletingStaleUrlsTests
{
    private readonly char[] _codeToDelete = "a1Bc12".ToCharArray();
    private readonly char[] _codeToKeep = "d4Ef56".ToCharArray();
    private ServiceProvider _scopedServices;
    private readonly StaleUrlsDeletingService _staleUrlsDeletingService;

    public DeletingStaleUrlsTests()
    {
        var sc = new ServiceCollection();
        var opts = new DbContextOptionsBuilder<UrlShortenerDbContext>();
        // won't work at this time, either try a docker container with a DB or SQLite
        // see extension RegisterStaleUrlsDeletingService
        opts.UseInMemoryDatabase("UrlShortener");
        var dbContext = new UrlShortenerDbContext(opts.Options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ShortenedUrls.Add(new ShortUrl
        {
            Code = Code.Create(_codeToKeep),
            CreatedOnUtc = DateTime.UtcNow,
            Short = Link.Create("https://localhost/" + new string(_codeToKeep)),
            Long = Link.Create("https://test.com")
        });
        dbContext.SaveChanges();
        dbContext.ShortenedUrls.Add(new ShortUrl
        {
            Code = Code.Create(_codeToDelete),
            CreatedOnUtc = DateTime.UtcNow.AddHours(-1),
            Short = Link.Create("https://localhost/" + new string(_codeToDelete)),
            Long = Link.Create("https://test2.com")
        });
        dbContext.SaveChanges();

        sc.AddScoped((IServiceProvider _) => dbContext);
        var logger = NullLogger<StaleUrlsDeletingService>.Instance;


        _scopedServices = sc.BuildServiceProvider();

        _staleUrlsDeletingService = new StaleUrlsDeletingService(logger, _scopedServices, new StaleConfigurationDeletingServiceConfig() { Interval = TimeSpan.FromSeconds(2) });
    }

    [Fact]
    public async Task PurgesStaleUrls()
    {
        await _staleUrlsDeletingService.StartAsync(CancellationToken.None);
        await Task.Delay(TimeSpan.FromSeconds(3));

        var dbContext = _scopedServices.GetRequiredService<UrlShortenerDbContext>();
        var allEntries = await dbContext.ShortenedUrls.ToListAsync();
        Assert.Equal(1, allEntries.Count);
        Assert.Equal("https://test.com", allEntries.First().Long.url);
    }
}
