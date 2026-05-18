namespace PowerTraderExam.Application.DTOs.Sync;

public class SyncCandidateDto
{
    public long Id { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string CandidateNo { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IdCard { get; set; }
    public string? OrgName { get; set; }
}
