using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;

namespace PowerTraderExam.Infrastructure.HostedServices;

public class ScorePushBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JavaPushOptions _options;
    private readonly ILogger<ScorePushBackgroundService> _logger;

    public ScorePushBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<JavaPushOptions> options,
        ILogger<ScorePushBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EnableBackgroundPush)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_options.RetryIntervalMinutes), stoppingToken);
                using var scope = _scopeFactory.CreateScope();
                var pushService = scope.ServiceProvider.GetRequiredService<IJavaScorePushService>();
                var result = await pushService.PushPendingScoresAsync(null, null, null, stoppingToken);
                if (result.SuccessCount + result.FailedCount > 0)
                {
                    _logger.LogInformation("Background push finished: success={Success}, failed={Failed}",
                        result.SuccessCount, result.FailedCount);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background score push failed");
            }
        }
    }
}
