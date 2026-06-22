using Microsoft.EntityFrameworkCore;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Repositories;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly PaperServiceDbContext _context;

    public BookmarkRepository(PaperServiceDbContext context) => _context = context;

    public async Task<IReadOnlyList<Bookmark>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Bookmarks
            .AsNoTracking()
            .Include(b => b.Paper).ThenInclude(p => p.Journal)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Bookmark?> GetByUserAndPaperAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default) =>
        _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.PaperId == paperId, cancellationToken);

    public async Task<HashSet<Guid>> GetBookmarkedPaperIdsAsync(Guid userId, IEnumerable<Guid> paperIds, CancellationToken cancellationToken = default)
    {
        var ids = paperIds.ToList();
        if (ids.Count == 0) return [];

        var bookmarked = await _context.Bookmarks
            .AsNoTracking()
            .Where(b => b.UserId == userId && ids.Contains(b.PaperId))
            .Select(b => b.PaperId)
            .ToListAsync(cancellationToken);

        return bookmarked.ToHashSet();
    }

    public Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default) =>
        _context.Bookmarks.AddAsync(bookmark, cancellationToken).AsTask();

    public void Remove(Bookmark bookmark) => _context.Bookmarks.Remove(bookmark);
}
