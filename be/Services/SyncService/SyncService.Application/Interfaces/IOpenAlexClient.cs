using SyncService.Application.DTOs.OpenAlex;
using SyncService.Domain.Entities;

namespace SyncService.Application.Interfaces;

public interface IOpenAlexClient
{
    Task<OpenAlexWorksResponse> FetchWorksAsync(DataSource dataSource, int maxCount, CancellationToken cancellationToken = default);
}
