using SyncService.Application.Interfaces;
using SyncService.Application.Interfaces.Repositories;
using SyncService.Infrastructure.Persistence;
using SyncService.Infrastructure.Persistence.Repositories;

namespace SyncService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SyncDbContext _db;

    public UnitOfWork(SyncDbContext db)
    {
        _db = db;
        DataSources = new DataSourceRepository(db);
        SyncLogs = new SyncLogRepository(db);
    }

    public IDataSourceRepository DataSources { get; }
    public ISyncLogRepository SyncLogs { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
