namespace PowerTraderExam.Domain.Entities;

public class ScorePushLog
{
    public long Id { get; set; }
    public long ExamScoreId { get; set; }
    public int AttemptNo { get; set; }
    public bool Success { get; set; }
    public string? RequestBody { get; set; }
    public string? SignHeader { get; set; }
    public string? TimestampHeader { get; set; }
    public string? ResponseBody { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExamScore ExamScore { get; set; } = null!;
}
