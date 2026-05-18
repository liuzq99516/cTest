namespace PowerTraderExam.Domain.Enums;

/// <summary>成绩向 Java 第三方系统的推送状态。</summary>
public enum ScorePushStatus
{
    /// <summary>未推送。</summary>
    NotPushed = 0,

    /// <summary>推送中（防止并发重复推送）。</summary>
    Pushing = 1,

    /// <summary>已成功推送。</summary>
    Pushed = 2,

    /// <summary>推送失败，可由 Hangfire 定时任务或管理端手动重试。</summary>
    Failed = 3
}
