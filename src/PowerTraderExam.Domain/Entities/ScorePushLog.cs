namespace PowerTraderExam.Domain.Entities;

/// <summary>
/// 成绩推送日志。每次向 Java 第三方系统发起推送（含 Hangfire 重试）时写入一条，用于审计与排错。
/// </summary>
public class ScorePushLog
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>关联的考试成绩 ID。</summary>
    public long ExamScoreId { get; set; }

    /// <summary>该成绩的第几次推送尝试（从 1 递增）。</summary>
    public int AttemptNo { get; set; }

    /// <summary>本次推送是否成功。</summary>
    public bool Success { get; set; }

    /// <summary>请求体原始 JSON（推送数组中单条或多条）。</summary>
    public string? RequestBody { get; set; }

    /// <summary>请求头中的 RSA 签名（Base64）。</summary>
    public string? SignHeader { get; set; }

    /// <summary>请求头中的时间戳，参与签名明文拼接。</summary>
    public string? TimestampHeader { get; set; }

    /// <summary>Java 接口返回的响应体。</summary>
    public string? ResponseBody { get; set; }

    /// <summary>失败时的错误信息。</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>日志创建时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>关联的考试成绩。</summary>
    public ExamScore ExamScore { get; set; } = null!;
}
