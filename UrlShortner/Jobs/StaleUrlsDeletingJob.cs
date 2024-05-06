namespace UrlShortner.Jobs;
public class StaleUrlsDeletingJob : IHostedService
{
    private int executionCount = 0;
    private readonly ILogger<StaleUrlsDeletingJob> _logger;
    private readonly IServiceProvider _services;
    private readonly StaleConfigurationDeletingJobSettings _config;

    public StaleUrlsDeletingJob(ILogger<StaleUrlsDeletingJob> logger, IServiceProvider services, StaleConfigurationDeletingJobSettings config)
    {
        _logger = logger;
        _services = services;
        _config = config;
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_config.Interval);
        while (
            !stoppingToken.IsCancellationRequested && 
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            var cutoff = DateTime.UtcNow.Add(-_config.Interval);

            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();
            var deleted =
                await dbContext.ShortenedUrls.Where(x => x.CreatedOnUtc < cutoff)
                .ExecuteDeleteAsync(cancellationToken: stoppingToken);

            _logger.LogInformation($"Deleted {deleted} links.");
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stale url deleting service is stopping.");
        return Task.CompletedTask;
    }
}