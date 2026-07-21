using AutoMapper;
using PRN232ASM.TrendService.Application.DTOs;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Application.Services;

public class TrendQueryService : ITrendService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TrendQueryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AnalyticsResponse> GetAnalyticsAsync(CancellationToken cancellationToken = default)
    {
        var trends = await _unitOfWork.PublicationTrends.GetAllAsync(cancellationToken);

        var papersByYear = trends
            .GroupBy(t => t.Year)
            .Select(g => new YearCountDto { Year = g.Key, Count = g.Sum(x => x.PaperCount) })
            .OrderBy(x => x.Year)
            .ToList();

        var topKeyword = trends
            .GroupBy(t => t.Keyword)
            .Select(g => new { Keyword = g.Key, Count = g.Sum(x => x.PaperCount) })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        return new AnalyticsResponse
        {
            TotalPapers = trends.Sum(t => t.PaperCount),
            TotalKeywords = trends.Select(t => t.Keyword).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            YearFrom = trends.Count == 0 ? 0 : trends.Min(t => t.Year),
            YearTo = trends.Count == 0 ? 0 : trends.Max(t => t.Year),
            TopKeyword = topKeyword?.Keyword ?? string.Empty,
            TopKeywordCount = topKeyword?.Count ?? 0,
            PapersByYear = papersByYear
        };
    }

    public async Task<IReadOnlyList<TrendResponse>> GetTrendsAsync(string? keyword, int? year, CancellationToken cancellationToken = default)
    {
        var trends = await _unitOfWork.PublicationTrends.GetAsync(keyword, year, cancellationToken);
        return _mapper.Map<IReadOnlyList<TrendResponse>>(trends);
    }
}
