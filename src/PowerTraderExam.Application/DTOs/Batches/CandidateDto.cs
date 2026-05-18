namespace PowerTraderExam.Application.DTOs.Batches;

public class CandidateDto
{
    public long Id { get; set; }
    public string CandidateNo { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IdCard { get; set; }
    public string? OrgName { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
}
