using TrendService.Domain.Entities;

namespace TrendService.Application.Interfaces;

public interface ITrendRepository
{
    Task<IReadOnlyList<PublicationTrend>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PublicationTrend>> GetKeywordTrendsAsync(short? year, CancellationToken cancellationToken = default);
    Task<PublicationTrend?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken = default);
    Task<DateTime?> GetLatestCalculatedAtAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PublicationTrend trend, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<PublicationTrend> trends, CancellationToken cancellationToken = default);
    Task ClearAllAsync(CancellationToken cancellationToken = default);
}
