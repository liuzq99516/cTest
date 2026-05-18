namespace PowerTraderExam.Application.DTOs.Batches;

public class CandidateImportItem
{
    public string CandidateNo { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IdCard { get; set; }
    public string? OrgName { get; set; }
}
