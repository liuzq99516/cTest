namespace PowerTraderExam.Application.Interfaces;

/// <summary>
/// Hangfire 定时任务：自动重试向 Java 推送未成功成绩。
/// 由 <c>JavaPush:EnableBackgroundPush</c> 与 <c>RetryIntervalMinutes</c> 控制是否注册及执行频率。
/// </summary>
public interface IScorePushJob
{
    /// <summary>执行一次后台推送（通常调用 <see cref="IJavaScorePushService.PushPendingScoresAsync"/>）。</summary>
    /// <param name="cancellationToken">取消令牌。</param>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
