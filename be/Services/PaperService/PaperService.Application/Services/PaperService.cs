using Common.Exceptions;
using Common.Models;
using PaperService.Application.DTOs.Requests;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;

namespace PaperService.Application.Services;

public class PaperService : IPaperService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaperService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<PaperListItemResponse>> SearchAsync(
        SearchPaperRequest request,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 50 ? 10 : request.PageSize;

        var (items, totalCount) = await _unitOfWork.ResearchPapers.SearchAsync(
            request.Keyword,
            request.Author,
            request.Journal,
            request.TopicId,
            page,
            pageSize,
            cancellationToken);

        HashSet<Guid> bookmarkedIds = [];
        if (userId.HasValue && items.Count > 0)
        {
            bookmarkedIds = await _unitOfWork.Bookmarks.GetBookmarkedPaperIdsAsync(
                userId.Value,
                items.Select(p => p.Id),
                cancellationToken);
        }

        return new PagedResult<PaperListItemResponse>
        {
            Items = items.Select(p => MapListItem(p, bookmarkedIds.Contains(p.Id))).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaperDetailResponse> GetDetailAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.ResearchPapers.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Paper with id '{id}' was not found.");

        var isBookmarked = false;
        if (userId.HasValue)
        {
            var bookmark = await _unitOfWork.Bookmarks.GetByUserAndPaperAsync(userId.Value, id, cancellationToken);
            isBookmarked = bookmark is not null;
        }

        return MapDetail(paper, isBookmarked);
    }

    public async Task<IReadOnlyList<JournalListItemResponse>> GetJournalsAsync(CancellationToken cancellationToken = default)
    {
        var journals = await _unitOfWork.Journals.GetAllAsync(cancellationToken);
        return journals.Select(j => new JournalListItemResponse
        {
            Id = j.Id,
            Name = j.Name,
            Issn = j.Issn,
            Publisher = j.Publisher,
            PaperCount = j.Papers.Count
        }).ToList();
    }

    public async Task<IReadOnlyList<KeywordListItemResponse>> GetKeywordsAsync(CancellationToken cancellationToken = default)
    {
        var keywords = await _unitOfWork.Keywords.GetAllAsync(cancellationToken);
        return keywords.Select(k => new KeywordListItemResponse
        {
            Id = k.Id,
            Name = k.Name,
            PaperCount = k.PaperKeywords.Count
        }).ToList();
    }

    public async Task<IReadOnlyList<TopicListItemResponse>> GetTopicsAsync(CancellationToken cancellationToken = default)
    {
        var topics = await _unitOfWork.ResearchTopics.GetAllAsync(cancellationToken);
        return topics.Select(t => new TopicListItemResponse
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            PaperCount = t.PaperTopics.Count
        }).ToList();
    }

    public async Task<IReadOnlyList<AuthorSummaryResponse>> SearchAuthorsAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var authors = await _unitOfWork.Authors.SearchByNameAsync(query.Trim(), 10, cancellationToken);
        return authors.Select(a => new AuthorSummaryResponse
        {
            Id = a.Id,
            Name = a.Name,
            Affiliation = a.Affiliation,
            Order = 0
        }).ToList();
    }

    public async Task<IReadOnlyList<BookmarkResponse>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var bookmarks = await _unitOfWork.Bookmarks.GetByUserIdAsync(userId, cancellationToken);
        return bookmarks.Select(b => new BookmarkResponse
        {
            Id = b.Id,
            PaperId = b.PaperId,
            Title = b.Paper.Title,
            JournalName = b.Paper.Journal?.Name,
            PublishedYear = b.Paper.PublishedYear,
            CreatedAt = b.CreatedAt
        }).ToList();
    }

    public async Task AddBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.ResearchPapers.GetByIdWithDetailsAsync(paperId, cancellationToken)
            ?? throw new NotFoundException($"Paper with id '{paperId}' was not found.");

        var existing = await _unitOfWork.Bookmarks.GetByUserAndPaperAsync(userId, paperId, cancellationToken);
        if (existing is not null)
            throw new ValidationException("Paper is already bookmarked.");

        await _unitOfWork.Bookmarks.AddAsync(new Bookmark
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PaperId = paper.Id,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        var bookmark = await _unitOfWork.Bookmarks.GetByUserAndPaperAsync(userId, paperId, cancellationToken)
            ?? throw new NotFoundException("Bookmark not found.");

        _unitOfWork.Bookmarks.Remove(bookmark);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static PaperListItemResponse MapListItem(ResearchPaper paper, bool isBookmarked) =>
        new()
        {
            Id = paper.Id,
            Title = paper.Title,
            Abstract = paper.Abstract,
            Doi = paper.Doi,
            PublishedYear = paper.PublishedYear,
            CitationCount = paper.CitationCount,
            JournalName = paper.Journal?.Name,
            Authors = paper.PaperAuthors
                .OrderBy(pa => pa.AuthorOrder)
                .Select(pa => pa.Author.Name)
                .ToList(),
            Keywords = paper.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
            IsBookmarked = isBookmarked
        };

    private static PaperDetailResponse MapDetail(ResearchPaper paper, bool isBookmarked) =>
        new()
        {
            Id = paper.Id,
            Title = paper.Title,
            Abstract = paper.Abstract,
            Doi = paper.Doi,
            PublishedYear = paper.PublishedYear,
            PublishedDate = paper.PublishedDate,
            CitationCount = paper.CitationCount,
            Url = paper.Url,
            IsBookmarked = isBookmarked,
            Journal = paper.Journal is null ? null : new JournalSummaryResponse
            {
                Id = paper.Journal.Id,
                Name = paper.Journal.Name,
                Issn = paper.Journal.Issn,
                Publisher = paper.Journal.Publisher
            },
            Authors = paper.PaperAuthors
                .OrderBy(pa => pa.AuthorOrder)
                .Select(pa => new AuthorSummaryResponse
                {
                    Id = pa.Author.Id,
                    Name = pa.Author.Name,
                    Affiliation = pa.Author.Affiliation,
                    Order = pa.AuthorOrder
                }).ToList(),
            Keywords = paper.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
            Topics = paper.PaperTopics.Select(pt => new TopicSummaryResponse
            {
                Id = pt.Topic.Id,
                Name = pt.Topic.Name
            }).ToList()
        };
}
