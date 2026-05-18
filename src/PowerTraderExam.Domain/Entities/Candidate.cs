using PowerTraderExam.Domain.Enums;

namespace PowerTraderExam.Domain.Entities;

public class Candidate
{
    public long Id { get; set; }
    public long BatchId { get; set; }
    public string CandidateNo { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IdCard { get; set; }
    public string? OrgName { get; set; }
    public CandidateSyncStatus SyncStatus { get; set; } = CandidateSyncStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExamBatch Batch { get; set; } = null!;
    public ICollection<CandidateSyncLog> SyncLogs { get; set; } = new List<CandidateSyncLog>();
    public ICollection<ExamScore> Scores { get; set; } = new List<ExamScore>();
}
