using System.Text.Json.Serialization;

namespace PowerTraderExam.Application.DTOs.Java;

public class JavaGradeCallbackItemDto
{
    [JsonPropertyName("studentId")]
    public string StudentId { get; set; } = string.Empty;

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonPropertyName("situation")]
    public string Situation { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public decimal? Score { get; set; }

    [JsonPropertyName("answerUrl")]
    public string? AnswerUrl { get; set; }
}
