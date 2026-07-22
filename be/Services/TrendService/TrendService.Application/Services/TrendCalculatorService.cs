using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PRN232ASM.BuildingBlocks.Contracts.Trends;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;
using PRN232ASM.TrendService.Application.Interfaces;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Application.Services;

public class PaperServiceOptions
{
    public const string SectionName = "PaperService";
    public string BaseUrl { get; set; } = "http://localhost:5002";
}

public class TrendCalculatorService : ITrendCalculatorService
{
    private const double GrowthThresholdPercent = 10.0;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<PaperServiceOptions> _paperServiceOptions;
    private readonly IEventBus _eventBus;
    private readonly ILogger<TrendCalculatorService> _logger;

    public TrendCalculatorService(
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        IOptions<PaperServiceOptions> paperServiceOptions,
        IEventBus eventBus,
        ILogger<TrendCalculatorService> logger)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _paperServiceOptions = paperServiceOptions;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task IncrementFromPaperAsync(
        Guid? topicId,
        int publicationYear,
        IEnumerable<string> keywords,
        CancellationToken cancellationToken = default)
    {
        foreach (var keyword in keywords.Distinct(StringComparer.OrdinalIgnoreCase))
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RecalculateTrendsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting trend aggregation from PaperService");

        var client = _httpClientFactory.CreateClient("PaperService");
        var baseUrl = _paperServiceOptions.Value.BaseUrl.TrimEnd('/');
        var page = 1;
        const int pageSize = 100;
        var aggregated = new Dictionary<string, PublicationTrend>(StringComparer.OrdinalIgnoreCase);

        while (true)
        {
            var response = await client.GetFromJsonAsync<PaperApiEnvelope>(
                $"{baseUrl}/api/papers?page={page}&pageSize={pageSize}",
                cancellationToken);

            var items = response?.Data?.Items ?? [];
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

        await PublishTrendAlertsAsync(aggregated.Values, client, baseUrl, cancellationToken);
    }

    private async Task PublishTrendAlertsAsync(
        IEnumerable<PublicationTrend> trends,
        HttpClient client,
        string baseUrl,
        CancellationToken cancellationToken)
    {
        var keywordIds = await FetchKeywordIdsAsync(client, baseUrl, cancellationToken);
        if (keywordIds.Count == 0)
        {
            return;
        }

        var alerts = 0;
        foreach (var group in trends.GroupBy(t => t.Keyword, StringComparer.OrdinalIgnoreCase))
        {
            var years = group.OrderBy(t => t.Year).ToList();
            if (years.Count < 2)
            {
                continue;
            }

            var latest = years[^1];
            var previous = years[^2];
            if (previous.PaperCount <= 0)
            {
                continue;
            }

            var growth = (latest.PaperCount - previous.PaperCount) / (double)previous.PaperCount * 100;
            if (growth < GrowthThresholdPercent)
            {
                continue;
            }

            if (!keywordIds.TryGetValue(group.Key, out var keywordId))
            {
                continue;
            }

            await _eventBus.PublishAsync(new TrendUpdatedEvent
            {
                KeywordId = keywordId,
                Keyword = group.Key,
                GrowthPercent = growth,
                PaperCount = latest.PaperCount,
                Period = $"{previous.Year}-{latest.Year}"
            }, cancellationToken);
            alerts++;
        }

        _logger.LogInformation("Published {Count} trend update alerts", alerts);
    }

    private async Task<Dictionary<string, Guid>> FetchKeywordIdsAsync(
        HttpClient client,
        string baseUrl,
        CancellationToken cancellationToken)
    {
        var map = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var response = await client.GetFromJsonAsync<KeywordApiEnvelope>(
            $"{baseUrl}/api/keywords",
            cancellationToken);

        foreach (var keyword in response?.Data ?? [])
        {
            if (!string.IsNullOrWhiteSpace(keyword.Name) && keyword.Id != Guid.Empty)
            {
                map[keyword.Name] = keyword.Id;
            }
        }

        return map;
    }

    private sealed class KeywordApiEnvelope
    {
        [JsonPropertyName("data")]
        public List<KeywordApiItem> Data { get; set; } = [];
    }

    private sealed class KeywordApiItem
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class PaperApiEnvelope
    {
        [JsonPropertyName("data")]
        public PaperPagedData? Data { get; set; }
    }

    private sealed class PaperPagedData
    {
        [JsonPropertyName("items")]
        public List<PaperApiItem> Items { get; set; } = [];
    }

    private sealed class PaperApiItem
    {
        [JsonPropertyName("publicationYear")]
        public int PublicationYear { get; set; }

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = [];
    }
}
