using PowerTraderExam.Application.DTOs.Scores;

namespace PowerTraderExam.Application.Interfaces;

public interface IJavaScorePushService
{
    Task<PushResultDto> PushPendingScoresAsync(string? batchCode, string? subject, IReadOnlyList<long>? scoreIds, CancellationToken ct = default);
}
