using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Scores;

namespace PowerTraderExam.Application.Interfaces;

public interface IScoreService
{
    Task<long> SubmitScoreAsync(string serverId, SubmitScoreRequest request, CancellationToken ct = default);
    Task<PagedResult<ScoreDto>> QueryScoresAsync(string? batchCode, string? subject, string? pushStatus, int page, int pageSize, CancellationToken ct = default);
    Task<byte[]> ExportScoresAsync(string? batchCode, string? subject, string? pushStatus, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<PushResultDto> PushScoresAsync(PushScoresRequest request, CancellationToken ct = default);
}
