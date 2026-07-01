using SyncService.Application.DTOs;

namespace SyncService.Application.Interfaces;

public interface IOpenAlexSearchService
{
    Task<OpenAlexSearchResponse> SearchAsync(string query, short? year, bool openAccessOnly, bool paywalledOnly, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexSearchResponse> BrowseOpenAccessAsync(short? year, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexSearchResponse> BrowsePaywalledAsync(short? year, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexSearchResponse> BrowseAllAsync(short? year, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexPaperResult> GetWorkAsync(string openAlexId, CancellationToken cancellationToken = default);
    Task<OpenAlexPaperResult> ImportToLibraryAsync(string openAlexId, CancellationToken cancellationToken = default);
}
