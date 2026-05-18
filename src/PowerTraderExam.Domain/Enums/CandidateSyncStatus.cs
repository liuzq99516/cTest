namespace PowerTraderExam.Domain.Enums;

/// <summary>考生在全局维度上的同步状态（是否已被任一台考试服务器确认同步）。</summary>
public enum CandidateSyncStatus
{
    /// <summary>待同步：尚未被任何考试服务器确认。</summary>
    Pending = 0,

    /// <summary>已同步：至少一台考试服务器已确认。</summary>
    Synced = 1
}
