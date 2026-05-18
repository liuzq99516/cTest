using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Batches;

namespace PowerTraderExam.Application.Interfaces;

public interface IBatchService
{
    Task<ExamBatchDto> CreateBatchAsync(CreateBatchRequest request, CancellationToken ct = default);
    Task<ImportResultDto> ImportCandidatesAsync(string batchCode, ImportCandidatesRequest request, CancellationToken ct = default);
    Task<ImportResultDto> ImportCandidatesFromExcelAsync(string batchCode, Stream excelStream, CancellationToken ct = default);
    Task<PagedResult<CandidateDto>> GetCandidatesAsync(string batchCode, int page, int pageSize, CancellationToken ct = default);
}

public class ExamBatchDto
{
    public long Id { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public DateTime? ExamDate { get; set; }
}
