using PowerTraderExam.Domain.Enums;

namespace PowerTraderExam.Domain.Entities;

/// <summary>
/// 考生。由管理端按批次导入；考试服务器通过同步接口拉取并确认后，方可参与该服务器的考试与成绩回写。
/// </summary>
public class Candidate
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>所属考试批次 ID。</summary>
    public long BatchId { get; set; }

    /// <summary>准考证号，同一批次内唯一。</summary>
    public string CandidateNo { get; set; } = string.Empty;

    /// <summary>学员 ID，与 Java 第三方系统及成绩推送字段对应。</summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>姓名。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>身份证号（可选）。</summary>
    public string? IdCard { get; set; }

    /// <summary>所在单位（可选）。</summary>
    public string? OrgName { get; set; }

    /// <summary>同步状态：待同步或已至少被一台考试服务器确认同步。</summary>
    public CandidateSyncStatus SyncStatus { get; set; } = CandidateSyncStatus.Pending;

    /// <summary>创建时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>所属批次。</summary>
    public ExamBatch Batch { get; set; } = null!;

    /// <summary>各考试服务器对该考生的同步确认记录。</summary>
    public ICollection<CandidateSyncLog> SyncLogs { get; set; } = new List<CandidateSyncLog>();

    /// <summary>该考生的实操/理论成绩记录（按科目唯一）。</summary>
    public ICollection<ExamScore> Scores { get; set; } = new List<ExamScore>();
}
