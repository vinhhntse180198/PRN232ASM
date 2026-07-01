using SyncService.Application.DTOs;

namespace SyncService.Application.Interfaces;

public interface IDoiResolver
{
    Task<ResolvedPaperUrls> ResolveAsync(string doi, CancellationToken cancellationToken = default);
}
