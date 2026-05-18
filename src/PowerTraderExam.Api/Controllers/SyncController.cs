using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PowerTraderExam.Api.Auth;
using PowerTraderExam.Api.Extensions;
using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Sync;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;

namespace PowerTraderExam.Api.Controllers;

[ApiController]
[Route("api/sync")]
[Authorize(Roles = AuthRoles.ExamServer)]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly CandidateSyncOptions _syncOptions;

    public SyncController(ISyncService syncService, IOptions<CandidateSyncOptions> syncOptions)
    {
        _syncService = syncService;
        _syncOptions = syncOptions.Value;
    }

    [HttpGet("candidates")]
    public async Task<ActionResult<ApiResponse<SyncCandidatesResponse>>> GetCandidates(
        [FromQuery] string? batchCode,
        [FromQuery] int? limit,
        CancellationToken ct)
    {
        var requested = limit ?? _syncOptions.DefaultPullCount;
        var capped = requested > _syncOptions.MaxPullCount;
        var effectiveLimit = capped ? _syncOptions.MaxPullCount : requested;
        if (effectiveLimit < 1) effectiveLimit = 1;

        if (capped)
        {
            Response.Headers["X-Sync-Limit-Capped"] = "true";
        }

        var serverId = User.GetServerId();
        var data = await _syncService.GetPendingCandidatesAsync(serverId, batchCode, effectiveLimit, ct);
        return Ok(ApiResponse<SyncCandidatesResponse>.Ok(data));
    }

    [HttpPost("candidates/confirm")]
    public async Task<ActionResult<ApiResponse<ConfirmSyncResultDto>>> Confirm(
        [FromBody] ConfirmSyncRequest request,
        CancellationToken ct)
    {
        var serverId = User.GetServerId();
        var data = await _syncService.ConfirmSyncAsync(serverId, request, ct);
        return Ok(ApiResponse<ConfirmSyncResultDto>.Ok(data));
    }
}
