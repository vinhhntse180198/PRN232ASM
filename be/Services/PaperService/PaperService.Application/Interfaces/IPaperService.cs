using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.DTOs.Requests;
using PRN232ASM.PaperService.Application.DTOs.Responses;

namespace PRN232ASM.PaperService.Application.Interfaces;

public interface IPaperService
{
    Task<PagedResult<PaperSummaryResponse>> SearchAsync(SearchPaperRequest request, CancellationToken cancellationToken = default);
    Task<PaperDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaperDetailResponse> CreateAsync(CreatePaperRequest request, CancellationToken cancellationToken = default);
    Task<bool> ImportAsync(ImportPaperRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuthorResponse>> GetAuthorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalResponse>> GetJournalsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeywordResponse>> GetKeywordsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TopicResponse>> GetTopicsAsync(CancellationToken cancellationToken = default);
    Task<BookmarkResponse> AddBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
    Task RemoveBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookmarkResponse>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaperRecommendationResponse>> GetRecommendationsAsync(
        Guid paperId,
        int limit = 5,
        CancellationToken cancellationToken = default);
}
