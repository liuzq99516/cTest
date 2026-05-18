using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;

namespace PowerTraderExam.Infrastructure.Jobs;

public class ScorePushJob : IScorePushJob
{
    private readonly IJavaScorePushService _pushService;
    private readonly JavaPushOptions _options;
    private readonly ILogger<ScorePushJob> _logger;

    public ScorePushJob(
        IJavaScorePushService pushService,
        IOptions<JavaPushOptions> options,
        ILogger<ScorePushJob> logger)
    {
        _pushService = pushService;
        _options = options.Value;
        _logger = logger;
    }

    [DisplayName("推送待同步成绩到 Java 系统")]
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.EnableBackgroundPush)
        {
            return;
        }

        var result = await _pushService.PushPendingScoresAsync(null, null, null, cancellationToken);
        if (result.SuccessCount + result.FailedCount > 0)
        {
            _logger.LogInformation(
                "Hangfire score push finished: success={Success}, failed={Failed}",
                result.SuccessCount,
                result.FailedCount);
        }
    }
}
