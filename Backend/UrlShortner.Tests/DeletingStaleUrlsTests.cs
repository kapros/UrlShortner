using System.Dynamic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UrlShortner.DataAccess;
using UrlShortner.Domain;
using UrlShortner.Jobs;

namespace UrlShortner.Tests;
public class DeletingStaleUrlsTests
{
    private readonly char[] _codeToDelete = "a1Bc12".ToCharArray();
    private readonly char[] _codeToKeep = "d4Ef56".ToCharArray();
    private ServiceProvider _scopedServices;
    private SqliteConnection _connection;
    private readonly StaleUrlsDeletingJob _staleUrlsDeletingService;

    public DeletingStaleUrlsTests()
    {
        var sc = new ServiceCollection();
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var opts = new DbContextOptionsBuilder<UrlShortenerDbContext>();
        opts.UseSqlite(_connection);
        var dbContext = new UrlShortenerDbContext(opts.Options);
        dbContext.Database.EnsureCreated();
        dbContext.ShortenedUrls.Add(new ShortUrl
        {
            Code = Code.Create(_codeToKeep),
            CreatedOnUtc = DateTime.UtcNow.AddMinutes(5),
            Short = Link.Create("https://localhost/" + new string(_codeToKeep)),
            Long = Link.Create("https://test.com")
        });
        dbContext.ShortenedUrls.Add(new ShortUrl
        {
            Code = Code.Create(_codeToDelete),
            CreatedOnUtc = DateTime.UtcNow.AddHours(-1),
            Short = Link.Create("https://localhost/" + new string(_codeToDelete)),
            Long = Link.Create("https://test2.com")
        });
        dbContext.SaveChanges();

        sc.AddScoped((IServiceProvider _) => new UrlShortenerDbContext(opts.Options));
        var logger = NullLogger<StaleUrlsDeletingJob>.Instance;


        _scopedServices = sc.BuildServiceProvider();

        _staleUrlsDeletingService = new StaleUrlsDeletingJob(logger, _scopedServices, new StaleConfigurationDeletingJobSettings() { Interval = TimeSpan.FromSeconds(100) });
    }

    [Fact]
    public async Task PurgesStaleUrls()
    {
        // hacky way to gracefully invoke a cancellation
        // todo: look for better handling of start/stop so it actually does its job
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        await Task.WhenAny(new List<Task>() {
         _staleUrlsDeletingService.StartAsync(token),
        Task.Delay(TimeSpan.FromSeconds(2))
        });
    
        await _staleUrlsDeletingService.StopAsync(token);

    var opts = new DbContextOptionsBuilder<UrlShortenerDbContext>();
        opts.UseSqlite(_connection);
        var dbContext = new UrlShortenerDbContext(opts.Options);
        var allEntries = await dbContext.ShortenedUrls.ToListAsync();
        Assert.Equal(1, allEntries.Count);
        Assert.Equal("https://test.com", allEntries.First().Long.url);
    }
}
