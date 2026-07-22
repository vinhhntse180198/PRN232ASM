using Microsoft.EntityFrameworkCore;
using SyncService.Application.Interfaces.Repositories;
using SyncService.Domain.Entities;
using SyncService.Infrastructure.Persistence;

namespace SyncService.Infrastructure.Persistence.Repositories;

public class SyncLogRepository : ISyncLogRepository
{
    private readonly SyncDbContext _db;

    public SyncLogRepository(SyncDbContext db) => _db = db;

    public async Task<IReadOnlyList<SyncLog>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.SyncLogs
            .Include(x => x.DataSource)
            .AsNoTracking()
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(cancellationToken);

    public async Task<SyncLog?> GetLatestAsync(CancellationToken cancellationToken = default)
        => await _db.SyncLogs
            .Include(x => x.DataSource)
            .AsNoTracking()
            .OrderByDescending(x => x.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(SyncLog log, CancellationToken cancellationToken = default)
        => await _db.SyncLogs.AddAsync(log, cancellationToken);

    public Task UpdateAsync(SyncLog log, CancellationToken cancellationToken = default)
    {
        _db.SyncLogs.Update(log);
        return Task.CompletedTask;
    }
}
