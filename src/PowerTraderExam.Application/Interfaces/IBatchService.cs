using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Batches;

namespace PowerTraderExam.Application.Interfaces;

/// <summary>
/// 考试批次与考生名单服务。供管理端创建批次、导入名单（JSON/Excel）及分页查询考生。
/// </summary>
public interface IBatchService
{
    /// <summary>创建新的考试批次。</summary>
    /// <param name="request">批次编码、名称、考试日期等。</param>
    /// <param name="ct">取消令牌。</param>
    /// <returns>新建批次信息。</returns>
    Task<ExamBatchDto> CreateBatchAsync(CreateBatchRequest request, CancellationToken ct = default);

    /// <summary>通过 JSON 批量导入考生到指定批次。</summary>
    /// <param name="batchCode">批次编码。</param>
    /// <param name="request">考生列表。</param>
    /// <param name="ct">取消令牌。</param>
    /// <returns>导入结果（成功/跳过/失败条数等）。</returns>
    Task<ImportResultDto> ImportCandidatesAsync(string batchCode, ImportCandidatesRequest request, CancellationToken ct = default);

    /// <summary>通过 Excel 文件导入考生（列：准考证号、学员ID、姓名、身份证、单位）。</summary>
    /// <param name="batchCode">批次编码。</param>
    /// <param name="excelStream">Excel 文件流。</param>
    /// <param name="ct">取消令牌。</param>
    /// <returns>导入结果。</returns>
    Task<ImportResultDto> ImportCandidatesFromExcelAsync(string batchCode, Stream excelStream, CancellationToken ct = default);

    /// <summary>分页查询指定批次下的考生名单。</summary>
    /// <param name="batchCode">批次编码。</param>
    /// <param name="page">页码，从 1 开始。</param>
    /// <param name="pageSize">每页条数。</param>
    /// <param name="ct">取消令牌。</param>
    Task<PagedResult<CandidateDto>> GetCandidatesAsync(string batchCode, int page, int pageSize, CancellationToken ct = default);
}

/// <summary>考试批次简要信息（API 响应 DTO）。</summary>
public class ExamBatchDto
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>批次编码。</summary>
    public string BatchCode { get; set; } = string.Empty;

    /// <summary>批次名称。</summary>
    public string BatchName { get; set; } = string.Empty;

    /// <summary>计划考试日期。</summary>
    public DateTime? ExamDate { get; set; }
}
