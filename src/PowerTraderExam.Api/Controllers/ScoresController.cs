using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerTraderExam.Api.Auth;
using PowerTraderExam.Api.Extensions;
using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Scores;
using PowerTraderExam.Application.Interfaces;

namespace PowerTraderExam.Api.Controllers;

[ApiController]
[Route("api/scores")]
public class ScoresController : ControllerBase
{
    private readonly IScoreService _scoreService;

    public ScoresController(IScoreService scoreService) => _scoreService = scoreService;

    [HttpPost]
    [Authorize(Roles = AuthRoles.ExamServer)]
    public async Task<ActionResult<ApiResponse<object>>> Submit([FromBody] SubmitScoreRequest request, CancellationToken ct)
    {
        var serverId = User.GetServerId();
        var id = await _scoreService.SubmitScoreAsync(serverId, request, ct);
        return Ok(ApiResponse<object>.Ok(new { scoreId = id }));
    }

    [HttpGet]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResult<ScoreDto>>>> Query(
        [FromQuery] string? batchCode,
        [FromQuery] string? subject,
        [FromQuery] string? pushStatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var data = await _scoreService.QueryScoresAsync(batchCode, subject, pushStatus, page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<ScoreDto>>.Ok(data));
    }

    [HttpGet("export")]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<IActionResult> Export(
        [FromQuery] string? batchCode,
        [FromQuery] string? subject,
        [FromQuery] string? pushStatus,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct = default)
    {
        var bytes = await _scoreService.ExportScoresAsync(batchCode, subject, pushStatus, from, to, ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"scores_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpPost("push")]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PushResultDto>>> Push([FromBody] PushScoresRequest request, CancellationToken ct)
    {
        var data = await _scoreService.PushScoresAsync(request, ct);
        return Ok(ApiResponse<PushResultDto>.Ok(data));
    }
}
