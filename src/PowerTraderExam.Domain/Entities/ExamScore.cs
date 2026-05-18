using PowerTraderExam.Domain.Enums;

namespace PowerTraderExam.Domain.Entities;

/// <summary>
/// 考试成绩。由考试服务器回写；同一考生同一科目仅保留一条记录。支持向 Java 第三方系统推送及推送日志追溯。
/// </summary>
public class ExamScore
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>关联考生 ID。</summary>
    public long CandidateId { get; set; }

    /// <summary>学员 ID（冗余存储，便于查询与推送）。</summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>考试科目：实操（SKILL）或理论（THEORY）。</summary>
    public ExamSubject Subject { get; set; } = ExamSubject.SKILL;

    /// <summary>考试情况：正常参考或缺考。</summary>
    public ExamSituation Situation { get; set; } = ExamSituation.NORMAL;

    /// <summary>分数；缺考等场景可为空。</summary>
    public decimal? Score { get; set; }

    /// <summary>答卷或附件 URL，推送 Java 时使用。</summary>
    public string? AnswerUrl { get; set; }

    /// <summary>成绩明细 JSON（扩展字段，可选）。</summary>
    public string? DetailJson { get; set; }

    /// <summary>回写成绩的考试服务器标识（来自 API Key 鉴权）。</summary>
    public string? SourceServerId { get; set; }

    /// <summary>考试程序上报成绩的时间（UTC）。</summary>
    public DateTime ScoredAt { get; set; } = DateTime.UtcNow;

    /// <summary>向 Java 推送的状态。</summary>
    public ScorePushStatus PushStatus { get; set; } = ScorePushStatus.NotPushed;

    /// <summary>最近一次成功推送的时间（UTC）。</summary>
    public DateTime? PushedAt { get; set; }

    /// <summary>记录创建时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>记录最后更新时间（UTC）。</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>关联考生。</summary>
    public Candidate Candidate { get; set; } = null!;

    /// <summary>历次向 Java 推送的尝试日志。</summary>
    public ICollection<ScorePushLog> PushLogs { get; set; } = new List<ScorePushLog>();
}
