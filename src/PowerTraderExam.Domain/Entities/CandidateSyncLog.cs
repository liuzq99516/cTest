namespace PowerTraderExam.Domain.Entities;

/// <summary>
/// 考生同步日志。记录某台考试服务器已确认拉取并同步某考生；同一考生与同一服务器仅允许一条记录（唯一索引）。
/// 用于统计单服务器在单批次内的已同步人数（默认上限 200 人/批次/服务器）。
/// </summary>
public class CandidateSyncLog
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>已同步的考生 ID。</summary>
    public long CandidateId { get; set; }

    /// <summary>执行同步确认的考试服务器标识。</summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>确认同步时间（UTC）。</summary>
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

    /// <summary>备注（可选）。</summary>
    public string? Remark { get; set; }

    /// <summary>关联考生。</summary>
    public Candidate Candidate { get; set; } = null!;
}
