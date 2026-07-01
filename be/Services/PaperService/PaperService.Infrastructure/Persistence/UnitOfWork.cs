using PaperService.Application.Interfaces;
using PaperService.Infrastructure.Data;

namespace PaperService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PaperDbContext _context;

    public UnitOfWork(
        PaperDbContext context,
        IPaperRepository papers,
        IJournalRepository journals,
        IAuthorRepository authors,
        IKeywordRepository keywords,
        IBookmarkRepository bookmarks)
    {
        _context = context;
        Papers = papers;
        Journals = journals;
        Authors = authors;
        Keywords = keywords;
        Bookmarks = bookmarks;
    }

    public IPaperRepository Papers { get; }
    public IJournalRepository Journals { get; }
    public IAuthorRepository Authors { get; }
    public IKeywordRepository Keywords { get; }
    public IBookmarkRepository Bookmarks { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
