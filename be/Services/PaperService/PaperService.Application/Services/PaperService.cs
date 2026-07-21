using AutoMapper;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.PaperService.Application.DTOs.Requests;
using PRN232ASM.PaperService.Application.DTOs.Responses;
using PRN232ASM.PaperService.Application.Interfaces;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Services;

public class PaperService : IPaperService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPaperEventPublisher _eventPublisher;

    public PaperService(IUnitOfWork unitOfWork, IMapper mapper, IPaperEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<PagedResult<PaperSummaryResponse>> SearchAsync(SearchPaperRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

        var result = await _unitOfWork.ResearchPapers.SearchAsync(
            page,
            pageSize,
            request.Keyword,
            request.Author,
            request.Journal,
            cancellationToken);

        return new PagedResult<PaperSummaryResponse>
        {
            Items = _mapper.Map<IReadOnlyList<PaperSummaryResponse>>(result.Items),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<PaperDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.ResearchPapers.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ResearchPaper), id);

        return _mapper.Map<PaperDetailResponse>(paper);
    }

    public async Task<PaperDetailResponse> CreateAsync(CreatePaperRequest request, CancellationToken cancellationToken = default)
    {
        var journal = await _unitOfWork.Journals.GetByNameAsync(request.JournalName, cancellationToken);
        if (journal is null)
        {
            journal = new Journal
            {
                Id = Guid.NewGuid(),
                Name = request.JournalName,
                Issn = $"ISSN-{request.JournalName.GetHashCode():X8}",
                Publisher = "Academic Press"
            };
            await _unitOfWork.Journals.AddAsync(journal, cancellationToken);
        }

        var paper = new ResearchPaper
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Abstract = request.Abstract,
            Doi = request.Doi,
            PublicationYear = request.PublicationYear,
            CitationCount = request.CitationCount,
            JournalId = journal.Id,
            Journal = journal,
            CreatedAt = DateTime.UtcNow
        };

        var authorOrder = 1;
        foreach (var authorName in request.Authors.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var author = await _unitOfWork.Authors.GetByNameAsync(authorName, cancellationToken);
            if (author is null)
            {
                author = new Author
                {
                    Id = Guid.NewGuid(),
                    Name = authorName,
                    Affiliation = "Research Institute"
                };
                await _unitOfWork.Authors.AddAsync(author, cancellationToken);
            }

            paper.PaperAuthors.Add(new PaperAuthor
            {
                PaperId = paper.Id,
                AuthorId = author.Id,
                AuthorOrder = authorOrder++,
                Author = author
            });
        }

        foreach (var keywordName in request.Keywords.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var keyword = await _unitOfWork.Keywords.GetByNameAsync(keywordName, cancellationToken);
            if (keyword is null)
            {
                keyword = new Keyword { Id = Guid.NewGuid(), Name = keywordName };
                await _unitOfWork.Keywords.AddAsync(keyword, cancellationToken);
            }

            paper.PaperKeywords.Add(new PaperKeyword
            {
                PaperId = paper.Id,
                KeywordId = keyword.Id,
                Keyword = keyword
            });
        }

        ResearchTopic? primaryTopic = null;
        foreach (var topicName in request.Topics.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var topic = await _unitOfWork.ResearchTopics.GetByNameAsync(topicName, cancellationToken);
            if (topic is null)
            {
                topic = new ResearchTopic
                {
                    Id = Guid.NewGuid(),
                    Name = topicName,
                    Description = $"Research area: {topicName}"
                };
                await _unitOfWork.ResearchTopics.AddAsync(topic, cancellationToken);
            }

            primaryTopic ??= topic;
            paper.PaperTopics.Add(new PaperTopic
            {
                PaperId = paper.Id,
                TopicId = topic.Id,
                Topic = topic
            });
        }

        await _unitOfWork.ResearchPapers.AddAsync(paper, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishPaperCreatedAsync(new PaperCreatedEvent
        {
            PaperId = paper.Id,
            Title = paper.Title,
            PublicationYear = paper.PublicationYear,
            TopicId = primaryTopic?.Id,
            TopicName = primaryTopic?.Name,
            Keywords = paper.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
            Authors = paper.PaperAuthors.OrderBy(pa => pa.AuthorOrder).Select(pa => pa.Author.Name).ToList(),
            JournalName = journal.Name
        }, cancellationToken);

        return _mapper.Map<PaperDetailResponse>(paper);
    }

    public async Task<IReadOnlyList<AuthorResponse>> GetAuthorsAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _unitOfWork.Authors.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<AuthorResponse>>(authors);
    }

    public async Task<IReadOnlyList<JournalResponse>> GetJournalsAsync(CancellationToken cancellationToken = default)
    {
        var journals = await _unitOfWork.Journals.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<JournalResponse>>(journals);
    }

    public async Task<IReadOnlyList<KeywordResponse>> GetKeywordsAsync(CancellationToken cancellationToken = default)
    {
        var keywords = await _unitOfWork.Keywords.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<KeywordResponse>>(keywords);
    }

    public async Task<IReadOnlyList<TopicResponse>> GetTopicsAsync(CancellationToken cancellationToken = default)
    {
        var topics = await _unitOfWork.ResearchTopics.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TopicResponse>>(topics);
    }

    public async Task<BookmarkResponse> AddBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.ResearchPapers.GetByIdAsync(paperId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResearchPaper), paperId);

        var existing = await _unitOfWork.Bookmarks.GetAsync(userId, paperId, cancellationToken);
        if (existing is not null)
        {
            return _mapper.Map<BookmarkResponse>(existing);
        }

        var bookmark = new Bookmark
        {
            UserId = userId,
            PaperId = paperId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Bookmarks.AddAsync(bookmark, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var saved = await _unitOfWork.Bookmarks.GetAsync(userId, paperId, cancellationToken)
            ?? bookmark;

        return _mapper.Map<BookmarkResponse>(saved);
    }

    public async Task RemoveBookmarkAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        var bookmark = await _unitOfWork.Bookmarks.GetAsync(userId, paperId, cancellationToken)
            ?? throw new NotFoundException("Bookmark", $"{userId}:{paperId}");

        await _unitOfWork.Bookmarks.RemoveAsync(bookmark, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookmarkResponse>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var bookmarks = await _unitOfWork.Bookmarks.GetByUserAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<BookmarkResponse>>(bookmarks);
    }
}

public interface IPaperEventPublisher
{
    Task PublishPaperCreatedAsync(PaperCreatedEvent @event, CancellationToken cancellationToken = default);
}
