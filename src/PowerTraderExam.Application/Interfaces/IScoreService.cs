using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Scores;

namespace PowerTraderExam.Application.Interfaces;

/// <summary>
/// 考试成绩服务。考试服务器回写成绩；管理端查询、导出 Excel 及触发 Java 推送。
/// </summary>
public interface IScoreService
{
    /// <summary>
    /// 考试服务器回写单条成绩。按考生 <c>studentId</c> 与科目 upsert；需该服务器已同步该考生。
    /// </summary>
    /// <param name="serverId">回写方考试服务器标识（来自鉴权）。</param>
    /// <param name="request">学员 ID、科目、情况、分数、答卷 URL 等。</param>
    /// <param name="ct">取消令牌。</param>
    /// <returns>成绩记录主键。</returns>
    Task<long> SubmitScoreAsync(string serverId, SubmitScoreRequest request, CancellationToken ct = default);

    /// <summary>管理端分页查询成绩，可按批次、科目、推送状态筛选。</summary>
    Task<PagedResult<ScoreDto>> QueryScoresAsync(string? batchCode, string? subject, string? pushStatus, int page, int pageSize, CancellationToken ct = default);

    /// <summary>管理端导出成绩为 Excel，支持批次、科目、推送状态及时间范围筛选。</summary>
    Task<byte[]> ExportScoresAsync(string? batchCode, string? subject, string? pushStatus, DateTime? from, DateTime? to, CancellationToken ct = default);

    /// <summary>管理端手动触发向 Java 推送成绩；可按批次、科目或指定成绩 ID 列表筛选。</summary>
    Task<PushResultDto> PushScoresAsync(PushScoresRequest request, CancellationToken ct = default);
}
