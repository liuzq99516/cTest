namespace PowerTraderExam.Application.DTOs.Scores;

public class ScoreDto
{
    public long Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string CandidateNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BatchCode { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Situation { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public string? AnswerUrl { get; set; }
    public string PushStatus { get; set; } = string.Empty;
    public DateTime ScoredAt { get; set; }
    public DateTime? PushedAt { get; set; }
}
