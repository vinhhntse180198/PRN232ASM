using PRN232ASM.PaperService.Application.Interfaces;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

namespace PRN232ASM.PaperService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PaperServiceDbContext _context;

    public UnitOfWork(PaperServiceDbContext context)
    {
        _context = context;
        ResearchPapers = new ResearchPaperRepository(context);
        Authors = new AuthorRepository(context);
        Journals = new JournalRepository(context);
        Keywords = new KeywordRepository(context);
        ResearchTopics = new ResearchTopicRepository(context);
        Bookmarks = new BookmarkRepository(context);
    }

    public IResearchPaperRepository ResearchPapers { get; }
    public IAuthorRepository Authors { get; }
    public IJournalRepository Journals { get; }
    public IKeywordRepository Keywords { get; }
    public IResearchTopicRepository ResearchTopics { get; }
    public IBookmarkRepository Bookmarks { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
