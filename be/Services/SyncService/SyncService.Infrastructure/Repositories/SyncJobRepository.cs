using SyncService.Application.Interfaces;
using SyncService.Domain.Entities;
using SyncService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncService.Infrastructure.Repositories;

public class SyncJobRepository : ISyncJobRepository
{
    private readonly SyncDbContext _context;
    public SyncJobRepository(SyncDbContext context) => _context = context;

    public async Task<IReadOnlyList<SyncLog>> GetAllLogsAsync(CancellationToken cancellationToken = default)
        => await _context.SyncLogs.AsNoTracking()
            .Include(l => l.DataSource)
            .OrderByDescending(l => l.StartedAt)
            .ToListAsync(cancellationToken);

    public async Task<DataSource?> GetDataSourceByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.DataSources.FirstOrDefaultAsync(d => d.Name == name, cancellationToken);

    public async Task AddLogAsync(SyncLog log, CancellationToken cancellationToken = default)
        => await _context.SyncLogs.AddAsync(log, cancellationToken);

    public Task UpdateDataSourceAsync(DataSource source, CancellationToken cancellationToken = default)
    {
        _context.DataSources.Update(source);
        return Task.CompletedTask;
    }
}
