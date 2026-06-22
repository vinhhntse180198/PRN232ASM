using PaperService.Application.Interfaces.Repositories;

namespace PaperService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IResearchPaperRepository ResearchPapers { get; }
    IAuthorRepository Authors { get; }
    IJournalRepository Journals { get; }
    IKeywordRepository Keywords { get; }
    IResearchTopicRepository ResearchTopics { get; }
    IBookmarkRepository Bookmarks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
