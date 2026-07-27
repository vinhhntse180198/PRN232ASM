using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.Contracts.Trends;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using PRN232ASM.TrendService.Application.Interfaces;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Application.Services;

public class PaperServiceOptions
{
    public const string SectionName = "PaperService";

    /// <summary>gRPC base address of PaperService (e.g. http://paper-service:8080).</summary>
    public string GrpcUrl { get; set; } = "http://localhost:5002";

    /// <summary>Legacy REST base URL. Kept for backward compatibility; Trend aggregation uses gRPC.</summary>
    public string BaseUrl { get; set; } = "http://localhost:5002";
}

public class TrendCalculatorService : ITrendCalculatorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaperCatalogClient _paperCatalogClient;
    private readonly IOutboxWriter _outbox;
    private readonly ILogger<TrendCalculatorService> _logger;

    public TrendCalculatorService(
        IUnitOfWork unitOfWork,
        IPaperCatalogClient paperCatalogClient,
        IOutboxWriter outbox,
        ILogger<TrendCalculatorService> logger)
    {
        _unitOfWork = unitOfWork;
        _paperCatalogClient = paperCatalogClient;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task IncrementFromPaperAsync(
        Guid? topicId,
        int publicationYear,
        IEnumerable<string> keywords,
        CancellationToken cancellationToken = default)
    {
        var keywordList = keywords.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        foreach (var keyword in keywordList)
        {
            var trend = await _unitOfWork.PublicationTrends.GetByKeywordYearAsync(keyword, publicationYear, topicId, cancellationToken);
            if (trend is null)
            {
                trend = new PublicationTrend
                {
                    Id = Guid.NewGuid(),
                    Keyword = keyword,
                    Year = publicationYear,
                    PaperCount = 1,
                    TopicId = topicId
                };
                await _unitOfWork.PublicationTrends.AddAsync(trend, cancellationToken);
            }
            else
            {
                trend.PaperCount += 1;
            }
        }

        if (topicId is Guid tid)
        {
            var paperCount = 0;
            foreach (var keyword in keywordList)
            {
                var row = await _unitOfWork.PublicationTrends.GetByKeywordYearAsync(keyword, publicationYear, tid, cancellationToken);
                if (row is not null)
                    paperCount = Math.Max(paperCount, row.PaperCount);
            }

            await _outbox.EnqueueAsync(new TrendUpdatedEvent
            {
                TopicId = tid,
                TopicName = keywordList.FirstOrDefault() ?? tid.ToString(),
                GrowthPercent = 0,
                PaperCount = paperCount,
                Period = publicationYear.ToString()
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RecalculateTrendsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting trend aggregation from PaperService via gRPC");

        var page = 1;
        const int pageSize = 100;
        var aggregated = new Dictionary<string, PublicationTrend>(StringComparer.OrdinalIgnoreCase);

        while (true)
        {
            var items = await _paperCatalogClient.SearchPapersAsync(page, pageSize, cancellationToken);
            if (items.Count == 0)
            {
                break;
            }

            foreach (var paper in items)
            {
                foreach (var keyword in paper.Keywords.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var key = $"{keyword}|{paper.PublicationYear}";
                    if (!aggregated.TryGetValue(key, out var trend))
                    {
                        trend = new PublicationTrend
                        {
                            Id = Guid.NewGuid(),
                            Keyword = keyword,
                            Year = paper.PublicationYear,
                            PaperCount = 0,
                            TopicId = null
                        };
                        aggregated[key] = trend;
                    }

                    trend.PaperCount += 1;
                }
            }

            if (items.Count < pageSize)
            {
                break;
            }

            page++;
        }

        await _unitOfWork.PublicationTrends.RemoveAllAsync(cancellationToken);
        foreach (var trend in aggregated.Values)
        {
            await _unitOfWork.PublicationTrends.AddAsync(trend, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Trend aggregation completed with {Count} trend rows", aggregated.Count);
    }
}
