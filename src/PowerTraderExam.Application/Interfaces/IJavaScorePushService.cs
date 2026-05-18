using PowerTraderExam.Application.DTOs.Scores;

namespace PowerTraderExam.Application.Interfaces;

/// <summary>
/// Java 第三方成绩推送服务。调用 <c>POST /student/grade/callback/score</c>，
/// 使用 RSA 公钥加密签名（请求体 JSON + 时间戳），并记录推送日志、更新成绩推送状态。
/// </summary>
public interface IJavaScorePushService
{
    /// <summary>
    /// 推送待处理成绩：状态为 <see cref="Domain.Enums.ScorePushStatus.NotPushed"/> 或 <see cref="Domain.Enums.ScorePushStatus.Failed"/> 的记录。
    /// 可按批次、科目或指定成绩 ID 缩小范围；按配置分批发送。
    /// </summary>
    /// <param name="batchCode">可选，限定批次。</param>
    /// <param name="subject">可选，限定科目（SKILL/THEORY）。</param>
    /// <param name="scoreIds">可选，指定成绩主键列表；为 null 时按状态与筛选条件批量推送。</param>
    /// <param name="ct">取消令牌。</param>
    /// <returns>成功、失败、跳过条数等汇总。</returns>
    Task<PushResultDto> PushPendingScoresAsync(string? batchCode, string? subject, IReadOnlyList<long>? scoreIds, CancellationToken ct = default);
}
