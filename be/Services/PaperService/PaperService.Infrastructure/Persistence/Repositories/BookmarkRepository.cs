using Microsoft.EntityFrameworkCore;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly PaperServiceDbContext _context;

    public BookmarkRepository(PaperServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        await _context.Bookmarks.AddAsync(bookmark, cancellationToken);
    }

    public async Task<Bookmark?> GetAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookmarks
            .Include(b => b.Paper)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.PaperId == paperId, cancellationToken);
    }

    public async Task<IReadOnlyList<Bookmark>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookmarks
            .Include(b => b.Paper)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task RemoveAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        _context.Bookmarks.Remove(bookmark);
        return Task.CompletedTask;
    }
}
