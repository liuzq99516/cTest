using PowerTraderExam.Application.DTOs.Sync;

namespace PowerTraderExam.Application.Interfaces;

/// <summary>
/// 考生同步服务。供多台考试服务器按批次拉取待同步考生并确认；
/// 单台服务器在同一批次内累计同步人数受配置上限约束（默认 200 人）。
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// 拉取当前服务器尚未同步的待同步考生列表。
    /// 实际返回条数不超过请求的 <paramref name="limit"/> 与本批次剩余配额的最小值。
    /// </summary>
    /// <param name="serverId">考试服务器标识。</param>
    /// <param name="batchCode">批次编码（必填）。</param>
    /// <param name="limit">单次拉取上限；为 null 时使用配置的默认值。</param>
    /// <param name="ct">取消令牌。</param>
    /// <returns>考生列表及本批次已同步人数、剩余配额等元数据。</returns>
    Task<SyncCandidatesResponse> GetPendingCandidatesAsync(string serverId, string? batchCode, int? limit, CancellationToken ct = default);

    /// <summary>
    /// 确认已将拉取的考生同步到本地考试程序。写入同步日志；超限或重复确认时由实现层校验并返回错误。
    /// </summary>
    /// <param name="serverId">考试服务器标识。</param>
    /// <param name="request">待确认的考生 ID 列表。</param>
    /// <param name="ct">取消令牌。</param>
    Task<ConfirmSyncResultDto> ConfirmSyncAsync(string serverId, ConfirmSyncRequest request, CancellationToken ct = default);
}
