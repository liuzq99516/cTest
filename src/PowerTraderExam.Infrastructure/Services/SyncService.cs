using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PowerTraderExam.Application.DTOs.Sync;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;
using PowerTraderExam.Domain.Entities;
using PowerTraderExam.Domain.Enums;
using PowerTraderExam.Infrastructure.Persistence;

namespace PowerTraderExam.Infrastructure.Services;

public class SyncService : ISyncService
{
    private readonly AppDbContext _db;
    private readonly CandidateSyncOptions _options;

    public SyncService(AppDbContext db, IOptions<CandidateSyncOptions> options)
    {
        _db = db;
        _options = options.Value;
    }

    public async Task<SyncCandidatesResponse> GetPendingCandidatesAsync(
        string serverId,
        string? batchCode,
        int? limit,
        CancellationToken ct = default)
    {
        var effectiveLimit = ResolveLimit(limit);

        var syncedIds = _db.CandidateSyncLogs
            .Where(x => x.ServerId == serverId)
            .Select(x => x.CandidateId);

        var query = _db.Candidates
            .Include(x => x.Batch)
            .Where(x => !syncedIds.Contains(x.Id));

        if (!string.IsNullOrWhiteSpace(batchCode))
        {
            query = query.Where(x => x.Batch.BatchCode == batchCode);
        }

        var items = await query
            .OrderBy(x => x.Id)
            .Take(effectiveLimit)
            .Select(x => new SyncCandidateDto
            {
                Id = x.Id,
                BatchCode = x.Batch.BatchCode,
                CandidateNo = x.CandidateNo,
                StudentId = x.StudentId,
                Name = x.Name,
                IdCard = x.IdCard,
                OrgName = x.OrgName
            })
            .ToListAsync(ct);

        return new SyncCandidatesResponse
        {
            Items = items,
            Count = items.Count,
            Limit = effectiveLimit,
            HasMore = items.Count == effectiveLimit
        };
    }

    public async Task<ConfirmSyncResultDto> ConfirmSyncAsync(
        string serverId,
        ConfirmSyncRequest request,
        CancellationToken ct = default)
    {
        if (request.CandidateIds.Count == 0)
        {
            return new ConfirmSyncResultDto();
        }

        if (request.CandidateIds.Count > _options.MaxConfirmCount)
        {
            throw new InvalidOperationException($"单次确认同步人数不能超过 {_options.MaxConfirmCount} 人。");
        }

        var confirmed = 0;
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        foreach (var candidateId in request.CandidateIds.Distinct())
        {
            var exists = await _db.CandidateSyncLogs
                .AnyAsync(x => x.CandidateId == candidateId && x.ServerId == serverId, ct);
            if (exists) continue;

            _db.CandidateSyncLogs.Add(new CandidateSyncLog
            {
                CandidateId = candidateId,
                ServerId = serverId,
                Remark = request.Remark
            });
            confirmed++;

            var candidate = await _db.Candidates.FindAsync(new object[] { candidateId }, ct);
            if (candidate != null)
            {
                candidate.SyncStatus = CandidateSyncStatus.Synced;
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new ConfirmSyncResultDto { Confirmed = confirmed };
    }

    private int ResolveLimit(int? limit)
    {
        var value = limit ?? _options.DefaultPullCount;
        if (value < 1) value = 1;
        if (value > _options.MaxPullCount) value = _options.MaxPullCount;
        return value;
    }
}
