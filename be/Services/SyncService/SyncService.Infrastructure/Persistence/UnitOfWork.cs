using SyncService.Application.Interfaces;
using SyncService.Infrastructure.Data;

namespace SyncService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SyncDbContext _context;
    public UnitOfWork(SyncDbContext context, ISyncJobRepository syncJobs) { _context = context; SyncJobs = syncJobs; }
    public ISyncJobRepository SyncJobs { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    public void Dispose() => _context.Dispose();
}
