using SyncService.Application.DTOs;

namespace SyncService.Application.Interfaces;

public interface ISyncService
{
    Task<IReadOnlyList<DataSourceDto>> GetDataSourcesAsync(CancellationToken cancellationToken = default);
    Task<DataSourceDto> UpdateDataSourceAsync(Guid id, UpdateDataSourceRequest request, CancellationToken cancellationToken = default);
    Task<SyncLogDto> TriggerSyncAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SyncLogDto>> GetLogsAsync(CancellationToken cancellationToken = default);
    Task<SyncStatusDto> GetStatusAsync(CancellationToken cancellationToken = default);
}
