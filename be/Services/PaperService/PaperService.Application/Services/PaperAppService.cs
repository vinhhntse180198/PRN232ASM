using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;

namespace PaperService.Application.Services;

public class PaperAppService : IPaperService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaperAppService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResult<PaperResponse>> SearchAsync(
        SearchPaperRequest request,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, total) = await _unitOfWork.Papers.SearchAsync(
            request.Query?.Trim(),
            request.Keyword?.Trim(),
            request.Doi?.Trim(),
            request.Year,
            page,
            pageSize,
            cancellationToken);

        var bookmarkedIds = userId.HasValue
            ? await _unitOfWork.Bookmarks.GetBookmarkedPaperIdsAsync(userId.Value, items.Select(p => p.Id), cancellationToken)
            : [];

        return new PagedResult<PaperResponse>
        {
            Items = items.Select(p => PaperMapper.Map(p, bookmarkedIds.Contains(p.Id))).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaperResponse?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.Papers.GetByIdWithDetailsAsync(id, cancellationToken);
        if (paper is null) return null;

        var isBookmarked = userId.HasValue
            && await _unitOfWork.Bookmarks.ExistsAsync(userId.Value, id, cancellationToken);

        return PaperMapper.Map(paper, isBookmarked);
    }

    public async Task<PaperResponse> CreateAsync(CreatePaperRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new InvalidOperationException("Title is required.");

        var doi = request.Doi?.Trim();
        if (!string.IsNullOrEmpty(doi) && await _unitOfWork.Papers.ExistsByDoiAsync(doi, cancellationToken))
            throw new InvalidOperationException("A paper with this DOI already exists.");

        Journal? journal = null;
        if (!string.IsNullOrWhiteSpace(request.JournalName))
        {
            var journalName = request.JournalName.Trim();
            journal = await _unitOfWork.Journals.GetByNameAsync(journalName, cancellationToken);
            if (journal is null)
            {
                journal = new Journal
                {
                    Id = Guid.NewGuid(),
                    Name = journalName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Journals.AddAsync(journal, cancellationToken);
            }
        }

        var paper = new ResearchPaper
        {
            Id = Guid.NewGuid(),
            JournalId = journal?.Id,
            ExternalId = request.ExternalId?.Trim(),
            Title = request.Title.Trim(),
            Abstract = request.Abstract?.Trim(),
            Doi = doi,
            PublishedYear = request.PublishedYear,
            PublishedDate = request.PublishedDate,
            Url = request.Url?.Trim(),
            CitationCount = request.CitationCount is > 0 ? request.CitationCount.Value : 0,
            IsOpenAccess = request.IsOpenAccess ?? false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await AttachAuthorsAsync(paper, request.AuthorNames, cancellationToken);
        await AttachKeywordsAsync(paper, request.Keywords, cancellationToken);

        await _unitOfWork.Papers.AddAsync(paper, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Papers.GetByIdWithDetailsAsync(paper.Id, cancellationToken)
            ?? paper;

        return PaperMapper.Map(created, false);
    }

    private async Task AttachAuthorsAsync(ResearchPaper paper, List<string>? authorNames, CancellationToken cancellationToken)
    {
        if (authorNames is null || authorNames.Count == 0) return;

        short order = 1;
        foreach (var rawName in authorNames)
        {
            var name = rawName.Trim();
            if (string.IsNullOrEmpty(name)) continue;

            var author = await _unitOfWork.Authors.GetByNameAsync(name, cancellationToken);
            if (author is null)
            {
                author = new Author
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Authors.AddAsync(author, cancellationToken);
            }

            paper.PaperAuthors.Add(new PaperAuthor
            {
                PaperId = paper.Id,
                AuthorId = author.Id,
                AuthorOrder = order++
            });
        }
    }

    private async Task AttachKeywordsAsync(ResearchPaper paper, List<string>? keywords, CancellationToken cancellationToken)
    {
        if (keywords is null || keywords.Count == 0) return;

        foreach (var rawKeyword in keywords)
        {
            var name = rawKeyword.Trim();
            if (string.IsNullOrEmpty(name)) continue;

            var normalized = name.ToLowerInvariant();
            var keyword = await _unitOfWork.Keywords.GetByNameAsync(normalized, cancellationToken);
            if (keyword is null)
            {
                keyword = new Keyword
                {
                    Id = Guid.NewGuid(),
                    Name = normalized,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Keywords.AddAsync(keyword, cancellationToken);
            }

            paper.PaperKeywords.Add(new PaperKeyword
            {
                PaperId = paper.Id,
                KeywordId = keyword.Id
            });
        }
    }
}
