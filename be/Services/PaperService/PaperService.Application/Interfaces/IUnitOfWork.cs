namespace PaperService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPaperRepository Papers { get; }
    IJournalRepository Journals { get; }
    IAuthorRepository Authors { get; }
    IKeywordRepository Keywords { get; }
    IBookmarkRepository Bookmarks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
