using SyncService.Domain.Entities;

namespace SyncService.Application.Interfaces;

public interface ISyncJobRepository
{
    Task<IReadOnlyList<SyncLog>> GetAllLogsAsync(CancellationToken cancellationToken = default);
    Task<DataSource?> GetDataSourceByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddLogAsync(SyncLog log, CancellationToken cancellationToken = default);
    Task UpdateDataSourceAsync(DataSource source, CancellationToken cancellationToken = default);
}
