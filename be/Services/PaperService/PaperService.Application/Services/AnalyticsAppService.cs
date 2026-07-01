using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;

namespace PaperService.Application.Services;

public class AnalyticsAppService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analytics;

    public AnalyticsAppService(IAnalyticsRepository analytics) => _analytics = analytics;

    public Task<LibraryAnalyticsDto> GetLibraryAnalyticsAsync(short? year, CancellationToken cancellationToken = default)
        => _analytics.GetLibraryAnalyticsAsync(year, cancellationToken);

    public Task<IReadOnlyList<BookmarkKeywordDto>> GetBookmarkKeywordStatsAsync(Guid userId, CancellationToken cancellationToken = default)
        => _analytics.GetBookmarkKeywordStatsAsync(userId, cancellationToken);
}
