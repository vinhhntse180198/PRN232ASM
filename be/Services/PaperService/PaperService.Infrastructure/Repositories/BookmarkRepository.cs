using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaperService.Infrastructure.Repositories;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly PaperDbContext _context;
    public BookmarkRepository(PaperDbContext context) => _context = context;

    public async Task<IReadOnlyList<Bookmark>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Bookmarks
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .Include(b => b.Paper).ThenInclude(p => p.Journal)
            .Include(b => b.Paper).ThenInclude(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(b => b.Paper).ThenInclude(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
        => await _context.Bookmarks.AnyAsync(b => b.UserId == userId && b.PaperId == paperId, cancellationToken);

    public async Task<HashSet<Guid>> GetBookmarkedPaperIdsAsync(
        Guid userId,
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default)
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

    public async Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
        => await _context.Bookmarks.AddAsync(bookmark, cancellationToken);

    public async Task<bool> RemoveAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.PaperId == paperId, cancellationToken);

        if (bookmark is null) return false;

        _context.Bookmarks.Remove(bookmark);
        return true;
    }
}
