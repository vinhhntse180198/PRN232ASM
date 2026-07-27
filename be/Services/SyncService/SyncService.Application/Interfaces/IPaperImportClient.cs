using SyncService.Application.DTOs;

namespace SyncService.Application.Interfaces;

public interface IPaperImportClient
{
    /// <summary>
    /// Imports a paper into PaperService. Returns the created paper id, or null on conflict/failure.
    /// </summary>
    Task<Guid?> GetCreatedPaperIdAsync(PaperImportRequest request, CancellationToken cancellationToken = default);
}
