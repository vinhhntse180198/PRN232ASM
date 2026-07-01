using TrendService.Application.DTOs;
using TrendService.Application.Interfaces;
using TrendService.Domain.Entities;

namespace TrendService.Application.Services;

public class TrendAppService : ITrendService
{
    private const short MinChartYear = 2020;
    private const short MaxChartYear = 2026;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaperAnalyticsClient _paperClient;

    public TrendAppService(IUnitOfWork unitOfWork, IPaperAnalyticsClient paperClient)
    {
        _unitOfWork = unitOfWork;
        _paperClient = paperClient;
    }

    public async Task<IReadOnlyList<TrendResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var trends = await _unitOfWork.Trends.GetAllAsync(cancellationToken);
        return trends.Select(Map).ToList();
    }

    public async Task<TrendResponse?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        var trend = await _unitOfWork.Trends.GetByTopicIdAsync(topicId, cancellationToken);
        return trend is null ? null : Map(trend);
    }

    public async Task<TrendResponse> CreateAsync(CreateTrendRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TopicId is null && request.KeywordId is null && string.IsNullOrWhiteSpace(request.KeywordName))
            throw new InvalidOperationException("TopicId, KeywordId, or KeywordName is required.");

        var trend = new PublicationTrend
        {
            Id = Guid.NewGuid(),
            KeywordId = request.KeywordId,
            KeywordName = request.KeywordName?.Trim(),
            TopicId = request.TopicId,
            Year = request.Year,
            PaperCount = request.PaperCount,
            CitationSum = request.CitationSum,
            GrowthRate = request.GrowthRate,
            CalculatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Trends.AddAsync(trend, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(trend);
    }

    public async Task<TrendRefreshResultDto> RefreshFromLibraryAsync(CancellationToken cancellationToken = default)
    {
        var analytics = await _paperClient.GetLibraryAnalyticsAsync(null, cancellationToken);
        var calculatedAt = DateTime.UtcNow;
        var trends = new List<PublicationTrend>();

        foreach (var group in analytics.KeywordYearStats.GroupBy(x => x.Keyword, StringComparer.OrdinalIgnoreCase))
        {
            var byYear = group.ToDictionary(x => x.Year);
            foreach (var stat in group)
            {
                var prevCount = byYear.TryGetValue((short)(stat.Year - 1), out var prev) ? prev.PaperCount : 0;
                trends.Add(new PublicationTrend
                {
                    Id = Guid.NewGuid(),
                    KeywordName = stat.Keyword,
                    Year = stat.Year,
                    PaperCount = stat.PaperCount,
                    CitationSum = stat.CitationSum,
                    GrowthRate = TrendBadgeHelper.ComputeGrowthRate(stat.PaperCount, prevCount),
                    CalculatedAt = calculatedAt
                });
            }
        }

        await _unitOfWork.Trends.ClearAllAsync(cancellationToken);
        if (trends.Count > 0)
            await _unitOfWork.Trends.AddRangeAsync(trends, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TrendRefreshResultDto
        {
            TrendsUpdated = trends.Count,
            CalculatedAt = calculatedAt
        };
    }

    public async Task<TrendsDashboardDto> GetDashboardAsync(
        short? year,
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        var stored = await _unitOfWork.Trends.GetKeywordTrendsAsync(year, cancellationToken);
        var lastRefreshed = await _unitOfWork.Trends.GetLatestCalculatedAtAsync(cancellationToken);

        if (stored.Count == 0)
        {
            await RefreshFromLibraryAsync(cancellationToken);
            stored = await _unitOfWork.Trends.GetKeywordTrendsAsync(year, cancellationToken);
            lastRefreshed = await _unitOfWork.Trends.GetLatestCalculatedAtAsync(cancellationToken);
        }

        var analytics = await _paperClient.GetLibraryAnalyticsAsync(null, cancellationToken);
        var keywordItems = BuildKeywordItems(stored, year);
        var bookmarkKeywords = await _paperClient.GetBookmarkKeywordStatsAsync(bearerToken, cancellationToken);

        return new TrendsDashboardDto
        {
            TotalPapers = analytics.TotalPapers,
            FilterYear = year,
            LastRefreshedAt = lastRefreshed,
            TopKeywords = keywordItems
                .OrderByDescending(x => x.PaperCount)
                .Take(20)
                .ToList(),
            CitationsByKeyword = keywordItems
                .OrderByDescending(x => x.CitationSum)
                .Take(20)
                .ToList(),
            PapersByYear = analytics.PapersByYear
                .Where(x => x.Year >= MinChartYear && x.Year <= MaxChartYear)
                .OrderBy(x => x.Year)
                .ToList(),
            OpenAccessByYear = analytics.OpenAccessByYear
                .Where(x => x.Year >= MinChartYear && x.Year <= MaxChartYear)
                .OrderBy(x => x.Year)
                .ToList(),
            BookmarkKeywords = bookmarkKeywords
        };
    }

    private static List<KeywordTrendItemDto> BuildKeywordItems(IReadOnlyList<PublicationTrend> stored, short? year)
    {
        if (year.HasValue)
        {
            return stored
                .Where(t => !string.IsNullOrWhiteSpace(t.KeywordName))
                .Select(MapKeywordItem)
                .ToList();
        }

        return stored
            .Where(t => !string.IsNullOrWhiteSpace(t.KeywordName))
            .GroupBy(t => t.KeywordName!, StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var latest = g.OrderByDescending(x => x.Year).First();
                return new KeywordTrendItemDto
                {
                    Keyword = g.Key,
                    Year = latest.Year,
                    PaperCount = g.Sum(x => x.PaperCount),
                    CitationSum = g.Sum(x => x.CitationSum),
                    GrowthRate = latest.GrowthRate,
                    TrendBadge = TrendBadgeHelper.FromGrowthRate(latest.GrowthRate)
                };
            })
            .ToList();
    }

    private static KeywordTrendItemDto MapKeywordItem(PublicationTrend trend) => new()
    {
        Keyword = trend.KeywordName ?? string.Empty,
        Year = trend.Year,
        PaperCount = trend.PaperCount,
        CitationSum = trend.CitationSum,
        GrowthRate = trend.GrowthRate,
        TrendBadge = TrendBadgeHelper.FromGrowthRate(trend.GrowthRate)
    };

    private static TrendResponse Map(PublicationTrend trend) => new()
    {
        Id = trend.Id,
        KeywordId = trend.KeywordId,
        KeywordName = trend.KeywordName,
        TopicId = trend.TopicId,
        Year = trend.Year,
        PaperCount = trend.PaperCount,
        CitationSum = trend.CitationSum,
        GrowthRate = trend.GrowthRate,
        TrendBadge = TrendBadgeHelper.FromGrowthRate(trend.GrowthRate),
        CalculatedAt = trend.CalculatedAt
    };
}
