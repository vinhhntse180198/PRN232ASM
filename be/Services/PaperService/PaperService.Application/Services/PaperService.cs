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
    private readonly IRecommendationClient _recommendationClient;
    private readonly IPricingClient _pricingClient;
    private readonly IInferenceClient _inferenceClient;
    private readonly IInventoryClient _inventoryClient;
    private readonly IUserProfileClient _userProfileClient;

    public PaperService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPaperEventPublisher eventPublisher,
        IRecommendationClient recommendationClient,
        IPricingClient pricingClient,
        IInferenceClient inferenceClient,
        IInventoryClient inventoryClient,
        IUserProfileClient userProfileClient)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
        _recommendationClient = recommendationClient;
        _pricingClient = pricingClient;
        _inferenceClient = inferenceClient;
        _inventoryClient = inventoryClient;
        _userProfileClient = userProfileClient;
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
            JournalId = journal.Id,
            TopicIds = paper.PaperTopics.Select(pt => pt.TopicId).ToList(),
            KeywordIds = paper.PaperKeywords.Select(pk => pk.KeywordId).ToList(),
            Keywords = paper.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
            Authors = paper.PaperAuthors.OrderBy(pa => pa.AuthorOrder).Select(pa => pa.Author.Name).ToList(),
            JournalName = journal.Name
        }, cancellationToken);

        return _mapper.Map<PaperDetailResponse>(paper);
    }

    public async Task<bool> ImportAsync(ImportPaperRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.ResearchPapers.ExistsByDoiOrTitleAsync(request.Doi, request.Title, cancellationToken))
            return false;

        await CreateAsync(new CreatePaperRequest
        {
            Title = request.Title,
            Abstract = request.Abstract ?? string.Empty,
            Doi = request.Doi ?? string.Empty,
            PublicationYear = request.PublicationYear ?? 0,
            CitationCount = request.CitationCount,
            JournalName = string.IsNullOrWhiteSpace(request.JournalName) ? "Unknown" : request.JournalName,
            Authors = request.AuthorNames,
            Keywords = request.Keywords,
            Topics = request.Topics
        }, cancellationToken);

        return true;
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

    public async Task<IReadOnlyList<PaperRecommendationResponse>> GetRecommendationsAsync(
        Guid paperId,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var source = await _unitOfWork.ResearchPapers.GetByIdAsync(paperId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResearchPaper), paperId);

        var allPapers = await _unitOfWork.ResearchPapers.GetAllWithDetailsAsync(cancellationToken);
        var candidates = allPapers
            .Where(p => p.Id != paperId)
            .Take(200)
            .Select(p => new RecommendationCandidate(
                p.Id,
                p.Title,
                p.PublicationYear,
                p.Journal?.Name ?? string.Empty,
                p.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
                p.PaperTopics.Select(pt => pt.Topic.Name).ToList()))
            .ToList();

        var ranked = await _recommendationClient.GetRecommendationsAsync(
            new RecommendationQuery(
                source.Id,
                limit <= 0 ? 5 : Math.Min(limit, 20),
                source.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
                source.PaperTopics.Select(pt => pt.Topic.Name).ToList(),
                source.Journal?.Name ?? string.Empty,
                source.PublicationYear,
                candidates),
            cancellationToken);

        var byId = allPapers.ToDictionary(p => p.Id);
        var results = new List<PaperRecommendationResponse>();

        foreach (var item in ranked)
        {
            if (!byId.TryGetValue(item.PaperId, out var paper))
                continue;

            results.Add(new PaperRecommendationResponse
            {
                PaperId = paper.Id,
                Title = paper.Title,
                JournalName = paper.Journal?.Name ?? string.Empty,
                PublicationYear = paper.PublicationYear,
                Authors = paper.PaperAuthors
                    .OrderBy(pa => pa.AuthorOrder)
                    .Select(pa => pa.Author.Name)
                    .ToList(),
                Score = item.Score,
                Reason = item.Reason
            });
        }

        return results;
    }

    public async Task<PaperImpactScoreResponse> GetImpactScoreAsync(Guid paperId, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.ResearchPapers.GetByIdAsync(paperId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResearchPaper), paperId);

        var scored = await _pricingClient.ComputeImpactScoreAsync(
            new ImpactScoreQuery(
                paper.Id,
                paper.Title,
                paper.PublicationYear,
                paper.CitationCount,
                paper.PaperKeywords.Count,
                paper.PaperTopics.Count,
                paper.PaperAuthors.Count,
                paper.Journal?.Name ?? string.Empty),
            cancellationToken);

        return new PaperImpactScoreResponse
        {
            PaperId = paper.Id,
            Title = paper.Title,
            Score = scored.Score,
            CurrencyLabel = scored.CurrencyLabel,
            Tier = scored.Tier,
            Explanation = scored.Explanation
        };
    }

    public async Task<PaperInsightResponse> GetInsightsAsync(Guid paperId, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.ResearchPapers.GetByIdAsync(paperId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResearchPaper), paperId);

        var insight = await _inferenceClient.InferInsightsAsync(
            new InsightQuery(
                paper.Id,
                paper.Title,
                paper.Abstract ?? string.Empty,
                paper.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
                paper.PaperTopics.Select(pt => pt.Topic.Name).ToList()),
            cancellationToken);

        return new PaperInsightResponse
        {
            PaperId = paper.Id,
            SuggestedKeywords = insight.SuggestedKeywords,
            SuggestedTopics = insight.SuggestedTopics,
            Confidence = insight.Confidence,
            Summary = insight.Summary
        };
    }

    public async Task<JournalCapacityResponse> GetJournalCapacityAsync(
        Guid journalId,
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        var journal = await _unitOfWork.Journals.GetByIdAsync(journalId, cancellationToken)
            ?? throw new NotFoundException(nameof(Journal), journalId);

        var targetYear = year is > 0 ? year.Value : DateTime.UtcNow.Year;
        var allPapers = await _unitOfWork.ResearchPapers.GetAllWithDetailsAsync(cancellationToken);
        var journalPapers = allPapers.Where(p => p.JournalId == journalId).ToList();
        var papersInYear = journalPapers.Count(p => p.PublicationYear == targetYear);

        var capacity = await _inventoryClient.GetCapacityAsync(
            new JournalCapacityQuery(
                journal.Id,
                journal.Name,
                targetYear,
                papersInYear,
                journalPapers.Count),
            cancellationToken);

        return new JournalCapacityResponse
        {
            JournalId = journal.Id,
            JournalName = journal.Name,
            Year = capacity.Year,
            Capacity = capacity.Capacity,
            Used = capacity.Used,
            Remaining = capacity.Remaining,
            Status = capacity.Status,
            Note = capacity.Note
        };
    }

    public async Task<ReadingProfileResponse> GetReadingProfileAsync(
        Guid userId,
        IReadOnlyList<string>? followedKeywords = null,
        IReadOnlyList<string>? followedTopics = null,
        IReadOnlyList<string>? followedJournals = null,
        CancellationToken cancellationToken = default)
    {
        var bookmarks = await _unitOfWork.Bookmarks.GetByUserAsync(userId, cancellationToken);
        var signals = new List<BookmarkedPaperSignal>();

        foreach (var bookmark in bookmarks)
        {
            var paper = await _unitOfWork.ResearchPapers.GetByIdAsync(bookmark.PaperId, cancellationToken);
            if (paper is null) continue;

            signals.Add(new BookmarkedPaperSignal(
                paper.Id,
                paper.Title,
                paper.PaperKeywords.Select(pk => pk.Keyword.Name).ToList(),
                paper.PaperTopics.Select(pt => pt.Topic.Name).ToList(),
                paper.Journal?.Name ?? string.Empty,
                paper.PublicationYear));
        }

        var profile = await _userProfileClient.BuildReadingProfileAsync(
            new ReadingProfileQuery(
                userId,
                signals,
                followedKeywords ?? Array.Empty<string>(),
                followedTopics ?? Array.Empty<string>(),
                followedJournals ?? Array.Empty<string>()),
            cancellationToken);

        return new ReadingProfileResponse
        {
            UserId = profile.UserId,
            BookmarkCount = profile.BookmarkCount,
            TopKeywords = profile.TopKeywords.Select(x => new InterestItemResponse { Name = x.Name, Weight = x.Weight }).ToList(),
            TopTopics = profile.TopTopics.Select(x => new InterestItemResponse { Name = x.Name, Weight = x.Weight }).ToList(),
            TopJournals = profile.TopJournals.Select(x => new InterestItemResponse { Name = x.Name, Weight = x.Weight }).ToList(),
            PersonaLabel = profile.PersonaLabel,
            Summary = profile.Summary
        };
    }
}

public interface IPaperEventPublisher
{
    Task PublishPaperCreatedAsync(PaperCreatedEvent @event, CancellationToken cancellationToken = default);
}
