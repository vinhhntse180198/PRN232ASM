using TrendService.Application.DTOs;

namespace TrendService.Application.Interfaces;

public interface ITrendService
{
    Task<IReadOnlyList<TrendResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TrendResponse?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken = default);
    Task<TrendResponse> CreateAsync(CreateTrendRequest request, CancellationToken cancellationToken = default);
    Task<TrendRefreshResultDto> RefreshFromLibraryAsync(CancellationToken cancellationToken = default);
    Task<TrendsDashboardDto> GetDashboardAsync(short? year, string? bearerToken, CancellationToken cancellationToken = default);
}
