using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Interfaces.Repositories;

public interface IBookmarkRepository
{
    Task<Bookmark?> GetAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Bookmark>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
    Task RemoveAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
}
