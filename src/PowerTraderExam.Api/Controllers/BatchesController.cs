using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerTraderExam.Api.Auth;
using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Batches;
using PowerTraderExam.Application.Interfaces;

namespace PowerTraderExam.Api.Controllers;

[ApiController]
[Route("api/batches")]
public class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;

    public BatchesController(IBatchService batchService) => _batchService = batchService;

    [HttpPost]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ExamBatchDto>>> Create([FromBody] CreateBatchRequest request, CancellationToken ct)
    {
        var data = await _batchService.CreateBatchAsync(request, ct);
        return Ok(ApiResponse<ExamBatchDto>.Ok(data));
    }

    [HttpPost("{batchCode}/candidates")]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportCandidates(
        string batchCode,
        [FromBody] ImportCandidatesRequest request,
        CancellationToken ct)
    {
        var data = await _batchService.ImportCandidatesAsync(batchCode, request, ct);
        return Ok(ApiResponse<ImportResultDto>.Ok(data));
    }

    [HttpPost("{batchCode}/candidates/import")]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportExcel(
        string batchCode,
        IFormFile file,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<ImportResultDto>.Fail(400, "请上传 Excel 文件。"));
        }

        await using var stream = file.OpenReadStream();
        var data = await _batchService.ImportCandidatesFromExcelAsync(batchCode, stream, ct);
        return Ok(ApiResponse<ImportResultDto>.Ok(data));
    }

    [HttpGet("{batchCode}/candidates")]
    [Authorize(Roles = AuthRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResult<CandidateDto>>>> GetCandidates(
        string batchCode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var data = await _batchService.GetCandidatesAsync(batchCode, page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<CandidateDto>>.Ok(data));
    }
}
