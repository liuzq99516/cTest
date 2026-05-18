using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Batches;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;
using PowerTraderExam.Domain.Entities;
using PowerTraderExam.Domain.Enums;
using PowerTraderExam.Infrastructure.Persistence;

namespace PowerTraderExam.Infrastructure.Services;

public class BatchService : IBatchService
{
    private readonly AppDbContext _db;

    public BatchService(AppDbContext db) => _db = db;

    public async Task<ExamBatchDto> CreateBatchAsync(CreateBatchRequest request, CancellationToken ct = default)
    {
        var existing = await _db.ExamBatches.FirstOrDefaultAsync(x => x.BatchCode == request.BatchCode, ct);
        if (existing != null)
        {
            return MapBatch(existing);
        }

        var batch = new ExamBatch
        {
            BatchCode = request.BatchCode,
            BatchName = request.BatchName,
            ExamDate = request.ExamDate,
            Status = 1
        };
        _db.ExamBatches.Add(batch);
        await _db.SaveChangesAsync(ct);
        return MapBatch(batch);
    }

    public async Task<ImportResultDto> ImportCandidatesAsync(string batchCode, ImportCandidatesRequest request, CancellationToken ct = default)
    {
        var batch = await GetOrCreateBatchAsync(batchCode, ct);
        var result = new ImportResultDto();

        foreach (var item in request.Candidates)
        {
            var existing = await _db.Candidates.FirstOrDefaultAsync(
                x => x.BatchId == batch.Id && x.CandidateNo == item.CandidateNo, ct);

            if (existing == null)
            {
                _db.Candidates.Add(new Candidate
                {
                    BatchId = batch.Id,
                    CandidateNo = item.CandidateNo,
                    StudentId = string.IsNullOrWhiteSpace(item.StudentId) ? item.CandidateNo : item.StudentId,
                    Name = item.Name,
                    IdCard = item.IdCard,
                    OrgName = item.OrgName,
                    SyncStatus = CandidateSyncStatus.Pending
                });
                result.Inserted++;
            }
            else
            {
                existing.Name = item.Name;
                existing.StudentId = string.IsNullOrWhiteSpace(item.StudentId) ? item.CandidateNo : item.StudentId;
                existing.IdCard = item.IdCard;
                existing.OrgName = item.OrgName;
                result.Updated++;
            }
        }

        await _db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<ImportResultDto> ImportCandidatesFromExcelAsync(string batchCode, Stream excelStream, CancellationToken ct = default)
    {
        var items = new List<CandidateImportItem>();
        using var workbook = new XLWorkbook(excelStream);
        var sheet = workbook.Worksheets.First();
        var rows = sheet.RangeUsed()?.RowsUsed().Skip(1);
        if (rows == null) return new ImportResultDto();

        foreach (var row in rows)
        {
            items.Add(new CandidateImportItem
            {
                CandidateNo = row.Cell(1).GetString().Trim(),
                StudentId = row.Cell(2).GetString().Trim(),
                Name = row.Cell(3).GetString().Trim(),
                IdCard = row.Cell(4).GetString().Trim(),
                OrgName = row.Cell(5).GetString().Trim()
            });
        }

        return await ImportCandidatesAsync(batchCode, new ImportCandidatesRequest { Candidates = items }, ct);
    }

    public async Task<PagedResult<CandidateDto>> GetCandidatesAsync(string batchCode, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Candidates.Include(x => x.Batch)
            .Where(x => x.Batch.BatchCode == batchCode);

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CandidateDto
            {
                Id = x.Id,
                CandidateNo = x.CandidateNo,
                StudentId = x.StudentId,
                Name = x.Name,
                IdCard = x.IdCard,
                OrgName = x.OrgName,
                SyncStatus = x.SyncStatus.ToString()
            })
            .ToListAsync(ct);

        return new PagedResult<CandidateDto> { Items = items, Page = page, PageSize = pageSize, Total = total };
    }

    private async Task<ExamBatch> GetOrCreateBatchAsync(string batchCode, CancellationToken ct)
    {
        var batch = await _db.ExamBatches.FirstOrDefaultAsync(x => x.BatchCode == batchCode, ct);
        if (batch != null) return batch;

        batch = new ExamBatch { BatchCode = batchCode, BatchName = batchCode, Status = 1 };
        _db.ExamBatches.Add(batch);
        await _db.SaveChangesAsync(ct);
        return batch;
    }

    private static ExamBatchDto MapBatch(ExamBatch batch) => new()
    {
        Id = batch.Id,
        BatchCode = batch.BatchCode,
        BatchName = batch.BatchName,
        ExamDate = batch.ExamDate
    };
}
