namespace PowerTraderExam.Application.DTOs.Scores;

public class SubmitScoreRequest
{
    public string StudentId { get; set; } = string.Empty;
    public string Subject { get; set; } = "SKILL";
    public string Situation { get; set; } = "NORMAL";
    public decimal? Score { get; set; }
    public string? AnswerUrl { get; set; }
    public string? DetailJson { get; set; }
    public DateTime? ScoredAt { get; set; }
}
