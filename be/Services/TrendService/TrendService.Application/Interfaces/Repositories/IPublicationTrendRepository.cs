using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Application.Interfaces.Repositories;

public interface IPublicationTrendRepository
{
    Task<IReadOnlyList<PublicationTrend>> GetAsync(string? keyword, int? year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PublicationTrend>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PublicationTrend?> GetByKeywordYearAsync(string keyword, int year, Guid? topicId, CancellationToken cancellationToken = default);
    Task AddAsync(PublicationTrend trend, CancellationToken cancellationToken = default);
    Task RemoveAllAsync(CancellationToken cancellationToken = default);
}
