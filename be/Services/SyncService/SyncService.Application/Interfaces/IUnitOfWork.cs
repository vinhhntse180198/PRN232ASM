using SyncService.Application.Interfaces.Repositories;

namespace SyncService.Application.Interfaces;

public interface IUnitOfWork
{
    IDataSourceRepository DataSources { get; }
    ISyncLogRepository SyncLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
