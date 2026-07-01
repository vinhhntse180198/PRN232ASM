using PaperService.Application.DTOs;

namespace PaperService.Application.Interfaces;

public interface IBookmarkService
{
    Task<IReadOnlyList<BookmarkResponse>> GetMyBookmarksAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BookmarkResponse> AddAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
}
