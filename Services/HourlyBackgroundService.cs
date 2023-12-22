namespace WebApplication2.Services;

public class HourlyBackgroundService : BackgroundService
{
    private ILogger<HourlyBackgroundService> _logger;

    public HourlyBackgroundService(ILogger<HourlyBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentTime = DateTime.UtcNow;
            if (currentTime is { Minute: 0, Second: 0 })
            {
                _logger.LogInformation("Updating VIP customers");
            }
        }
    }
}