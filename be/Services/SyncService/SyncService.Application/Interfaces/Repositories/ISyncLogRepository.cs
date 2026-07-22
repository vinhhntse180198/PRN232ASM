using SyncService.Domain.Entities;

namespace SyncService.Application.Interfaces.Repositories;

public interface ISyncLogRepository
{
    Task<IReadOnlyList<SyncLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SyncLog?> GetLatestAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SyncLog log, CancellationToken cancellationToken = default);
    Task UpdateAsync(SyncLog log, CancellationToken cancellationToken = default);
}
