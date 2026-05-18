namespace PowerTraderExam.Application.DTOs.Batches;

public class ImportCandidatesRequest
{
    public List<CandidateImportItem> Candidates { get; set; } = new();
}
