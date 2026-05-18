using PowerTraderExam.Domain.Enums;

namespace PowerTraderExam.Domain.Entities;

public class ExamScore
{
    public long Id { get; set; }
    public long CandidateId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public ExamSubject Subject { get; set; } = ExamSubject.SKILL;
    public ExamSituation Situation { get; set; } = ExamSituation.NORMAL;
    public decimal? Score { get; set; }
    public string? AnswerUrl { get; set; }
    public string? DetailJson { get; set; }
    public string? SourceServerId { get; set; }
    public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
    public ScorePushStatus PushStatus { get; set; } = ScorePushStatus.NotPushed;
    public DateTime? PushedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Candidate Candidate { get; set; } = null!;
    public ICollection<ScorePushLog> PushLogs { get; set; } = new List<ScorePushLog>();
}
