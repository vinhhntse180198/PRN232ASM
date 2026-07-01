using PaperService.Application.DTOs;

namespace PaperService.Application.Interfaces;

public interface IAnalyticsService
{
    Task<LibraryAnalyticsDto> GetLibraryAnalyticsAsync(short? year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookmarkKeywordDto>> GetBookmarkKeywordStatsAsync(Guid userId, CancellationToken cancellationToken = default);
}
