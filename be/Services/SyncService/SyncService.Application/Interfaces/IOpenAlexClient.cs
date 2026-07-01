using SyncService.Application.DTOs.OpenAlex;

namespace SyncService.Application.Interfaces;

public interface IOpenAlexClient
{
    Task<OpenAlexWorksResponse> FetchWorksByYearAsync(short year, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexWorksResponse> BrowseOpenAccessWorksAsync(short? year, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexWorksResponse> BrowsePaywalledWorksAsync(short? year, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexWorksResponse> BrowseAllWorksAsync(short? year, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexWorksResponse> SearchWorksAsync(string query, short? year, bool openAccessOnly, bool paywalledOnly, int page, int perPage, CancellationToken cancellationToken = default);
    Task<OpenAlexWork?> GetWorkByIdAsync(string openAlexId, CancellationToken cancellationToken = default);
}
