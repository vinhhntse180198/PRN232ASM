using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces;

public interface IBookmarkRepository
{
    Task<IReadOnlyList<Bookmark>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
    Task<HashSet<Guid>> GetBookmarkedPaperIdsAsync(Guid userId, IEnumerable<Guid> paperIds, CancellationToken cancellationToken = default);
    Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
}
