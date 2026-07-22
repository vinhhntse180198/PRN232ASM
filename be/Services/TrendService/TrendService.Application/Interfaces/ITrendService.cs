using PRN232ASM.TrendService.Application.DTOs;

namespace PRN232ASM.TrendService.Application.Interfaces;

public interface ITrendService
{
    Task<IReadOnlyList<TrendResponse>> GetTrendsAsync(string? keyword, int? year, CancellationToken cancellationToken = default);
    Task<AnalyticsResponse> GetAnalyticsAsync(CancellationToken cancellationToken = default);
}

public interface ITrendCalculatorService
{
    Task RecalculateTrendsAsync(CancellationToken cancellationToken = default);
    Task IncrementFromPaperAsync(Guid? topicId, int publicationYear, IEnumerable<string> keywords, CancellationToken cancellationToken = default);
}
