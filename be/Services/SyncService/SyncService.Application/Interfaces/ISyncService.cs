using SyncService.Application.DTOs;

namespace SyncService.Application.Interfaces;

public interface ISyncService
{
    Task<IReadOnlyList<SyncLogResponse>> GetStatusAsync(CancellationToken cancellationToken = default);
    Task<SyncLogResponse> RunSyncAsync(string sourceName, CancellationToken cancellationToken = default);
    Task<SyncLogResponse> RunSyncAsync(string sourceName, SyncRunOptions? options, CancellationToken cancellationToken = default);
}
