using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<PaperServiceOptions> _paperServiceOptions;
    private readonly ILogger<TrendCalculatorService> _logger;

    public TrendCalculatorService(
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        IOptions<PaperServiceOptions> paperServiceOptions,
        ILogger<TrendCalculatorService> logger)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _paperServiceOptions = paperServiceOptions;
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
