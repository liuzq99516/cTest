namespace PowerTraderExam.Application.DTOs.Scores;

public class PushScoresRequest
{
    public string? BatchCode { get; set; }
    public string? Subject { get; set; }
    public List<long>? ScoreIds { get; set; }
}
