using PaperService.Application.Interfaces;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Infrastructure.Persistence.Repositories;

namespace PaperService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PaperServiceDbContext _context;

    public UnitOfWork(PaperServiceDbContext context)
    {
        _context = context;
        ResearchPapers = new ResearchPaperRepository(_context);
        Authors = new AuthorRepository(_context);
        Journals = new JournalRepository(_context);
        Keywords = new KeywordRepository(_context);
        ResearchTopics = new ResearchTopicRepository(_context);
        Bookmarks = new BookmarkRepository(_context);
    }

    public IResearchPaperRepository ResearchPapers { get; }
    public IAuthorRepository Authors { get; }
    public IJournalRepository Journals { get; }
    public IKeywordRepository Keywords { get; }
    public IResearchTopicRepository ResearchTopics { get; }
    public IBookmarkRepository Bookmarks { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
