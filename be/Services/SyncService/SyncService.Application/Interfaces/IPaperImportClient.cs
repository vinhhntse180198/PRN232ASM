using SyncService.Application.DTOs;

namespace SyncService.Application.Interfaces;

public interface IPaperImportClient
{
    Task<PaperImportResult> ImportAsync(PaperImportRequest request, CancellationToken cancellationToken = default);
}

public enum PaperImportResult
{
    Created,
    SkippedDuplicate,
    Failed
}
