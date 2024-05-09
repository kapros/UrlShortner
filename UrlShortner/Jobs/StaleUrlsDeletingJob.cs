namespace UrlShortner.Jobs;
public class StaleUrlsDeletingJob : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<StaleUrlsDeletingJob> _logger;
    private readonly IServiceProvider _services;
    private readonly StaleConfigurationDeletingJobSettings _config;
    private Timer? _timer = null;

    public StaleUrlsDeletingJob(ILogger<StaleUrlsDeletingJob> logger, IServiceProvider services, StaleConfigurationDeletingJobSettings config)
    {
        _logger = logger;
        _services = services;
        _config = config;
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero,_config.Interval);

    }

    private void DoWork(object? state)
    {
        var cutoff = DateTime.UtcNow.Add(-_config.Interval);

        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();
        var deleted = dbContext.ShortenedUrls.Where(x => x.CreatedOnUtc < cutoff).ExecuteDelete();

        _logger.LogInformation($"Deleted {deleted} links.");
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stale url deleting service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}