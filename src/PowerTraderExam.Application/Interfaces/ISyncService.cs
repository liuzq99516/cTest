using PowerTraderExam.Application.DTOs.Sync;

namespace PowerTraderExam.Application.Interfaces;

public interface ISyncService
{
    Task<SyncCandidatesResponse> GetPendingCandidatesAsync(string serverId, string? batchCode, int? limit, CancellationToken ct = default);
    Task<ConfirmSyncResultDto> ConfirmSyncAsync(string serverId, ConfirmSyncRequest request, CancellationToken ct = default);
}
