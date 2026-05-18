namespace PowerTraderExam.Domain.Entities;

/// <summary>
/// 考试批次。管理端创建批次后，在该批次下导入考生名单；考试服务器按批次拉取并同步考生。
/// </summary>
public class ExamBatch
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>批次编码，全局唯一，用于 API 路径与查询参数。</summary>
    public string BatchCode { get; set; } = string.Empty;

    /// <summary>批次名称（展示用）。</summary>
    public string BatchName { get; set; } = string.Empty;

    /// <summary>计划考试日期（可选）。</summary>
    public DateTime? ExamDate { get; set; }

    /// <summary>批次状态（业务自定义数值，如启用/归档）。</summary>
    public int Status { get; set; }

    /// <summary>创建时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>该批次下的考生集合。</summary>
    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}
