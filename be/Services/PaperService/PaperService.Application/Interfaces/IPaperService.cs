using Common.Models;
using PaperService.Application.DTOs.Requests;
using PaperService.Application.DTOs.Responses;

namespace PaperService.Application.Interfaces;

public interface IPaperService
{
    Task<PagedResult<PaperListItemResponse>> SearchAsync(SearchPaperRequest request, Guid? userId, CancellationToken cancellationToken = default);
    Task<PaperDetailResponse> GetDetailAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalListItemResponse>> GetJournalsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeywordListItemResponse>> GetKeywordsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TopicListItemResponse>> GetTopicsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuthorSummaryResponse>> SearchAuthorsAsync(string query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookmarkResponse>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
    Task RemoveBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
}
