using TrendService.Application.DTOs;

namespace TrendService.Application.Interfaces;

public interface IPaperAnalyticsClient
{
    Task<PaperLibraryAnalyticsDto> GetLibraryAnalyticsAsync(short? year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookmarkKeywordItem>> GetBookmarkKeywordStatsAsync(string? bearerToken, CancellationToken cancellationToken = default);
}
