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
        if (string.IsNullOrWhiteSpace(batchCode))
        {
            throw new ArgumentException("batchCode 为必填参数，每个考试服务器按批次同步考生。");
        }

        var batch = await _db.ExamBatches.FirstOrDefaultAsync(x => x.BatchCode == batchCode, ct);
        if (batch == null)
        {
            return BuildEmptyResponse(batchCode, ResolveLimit(limit), 0);
        }

        var syncedInBatch = await CountSyncedInBatchAsync(serverId, batch.Id, ct);
        var remainingQuota = _options.MaxSyncPerBatchPerServer - syncedInBatch;

        if (remainingQuota <= 0)
        {
            return new SyncCandidatesResponse
            {
                BatchCode = batchCode,
                Items = Array.Empty<SyncCandidateDto>(),
                Count = 0,
                Limit = 0,
                HasMore = false,
                MaxSyncPerBatch = _options.MaxSyncPerBatchPerServer,
                SyncedCountInBatch = syncedInBatch,
                RemainingQuota = 0
            };
        }

        var pullLimit = ResolveLimit(limit);
        var effectiveLimit = Math.Min(pullLimit, remainingQuota);

        var syncedIds = _db.CandidateSyncLogs
            .Where(x => x.ServerId == serverId)
            .Select(x => x.CandidateId);

        var items = await _db.Candidates
            .Include(x => x.Batch)
            .Where(x => x.BatchId == batch.Id && !syncedIds.Contains(x.Id))
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

        var hasMorePending = await _db.Candidates
            .Where(x => x.BatchId == batch.Id && !syncedIds.Contains(x.Id))
            .CountAsync(ct) > items.Count;

        var hasMore = items.Count == effectiveLimit
            && hasMorePending
            && syncedInBatch + items.Count < _options.MaxSyncPerBatchPerServer;

        return new SyncCandidatesResponse
        {
            BatchCode = batchCode,
            Items = items,
            Count = items.Count,
            Limit = effectiveLimit,
            HasMore = hasMore,
            MaxSyncPerBatch = _options.MaxSyncPerBatchPerServer,
            SyncedCountInBatch = syncedInBatch,
            RemainingQuota = remainingQuota
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

        var candidateIds = request.CandidateIds.Distinct().ToList();
        var candidates = await _db.Candidates
            .Where(x => candidateIds.Contains(x.Id))
            .ToListAsync(ct);

        if (candidates.Count != candidateIds.Count)
        {
            throw new KeyNotFoundException("部分考生 ID 不存在。");
        }

        await ValidateBatchSyncQuotaAsync(serverId, candidates, ct);

        var confirmed = 0;
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        foreach (var candidateId in candidateIds)
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

            var candidate = candidates.First(c => c.Id == candidateId);
            candidate.SyncStatus = CandidateSyncStatus.Synced;
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new ConfirmSyncResultDto { Confirmed = confirmed };
    }

    private async Task ValidateBatchSyncQuotaAsync(
        string serverId,
        IReadOnlyList<Domain.Entities.Candidate> candidates,
        CancellationToken ct)
    {
        foreach (var batchGroup in candidates.GroupBy(x => x.BatchId))
        {
            var batchId = batchGroup.Key;
            var alreadySynced = await CountSyncedInBatchAsync(serverId, batchId, ct);

            var newSyncIds = new List<long>();
            foreach (var candidate in batchGroup)
            {
                var exists = await _db.CandidateSyncLogs
                    .AnyAsync(x => x.CandidateId == candidate.Id && x.ServerId == serverId, ct);
                if (!exists)
                {
                    newSyncIds.Add(candidate.Id);
                }
            }

            if (alreadySynced + newSyncIds.Count > _options.MaxSyncPerBatchPerServer)
            {
                var batchCode = await _db.ExamBatches
                    .Where(x => x.Id == batchId)
                    .Select(x => x.BatchCode)
                    .FirstAsync(ct);

                throw new InvalidOperationException(
                    $"考试服务器 [{serverId}] 在批次 [{batchCode}] 已累计同步 {alreadySynced} 人，" +
                    $"本次再同步 {newSyncIds.Count} 人将超过上限 {_options.MaxSyncPerBatchPerServer} 人。");
            }
        }
    }

    private Task<int> CountSyncedInBatchAsync(string serverId, long batchId, CancellationToken ct) =>
        _db.CandidateSyncLogs
            .Where(x => x.ServerId == serverId && x.Candidate.BatchId == batchId)
            .CountAsync(ct);

    private SyncCandidatesResponse BuildEmptyResponse(string batchCode, int limit, int syncedInBatch) =>
        new()
        {
            BatchCode = batchCode,
            Items = Array.Empty<SyncCandidateDto>(),
            Count = 0,
            Limit = limit,
            HasMore = false,
            MaxSyncPerBatch = _options.MaxSyncPerBatchPerServer,
            SyncedCountInBatch = syncedInBatch,
            RemainingQuota = Math.Max(0, _options.MaxSyncPerBatchPerServer - syncedInBatch)
        };

    private int ResolveLimit(int? limit)
    {
        var value = limit ?? _options.DefaultPullCount;
        if (value < 1) value = 1;
        if (value > _options.MaxPullCount) value = _options.MaxPullCount;
        return value;
    }
}
