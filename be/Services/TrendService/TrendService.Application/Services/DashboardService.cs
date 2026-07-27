using AutoMapper;
using PRN232ASM.TrendService.Application.DTOs;
using PRN232ASM.TrendService.Application.Interfaces;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var trends = await _unitOfWork.PublicationTrends.GetAllAsync(cancellationToken);

        var papersByYear = trends
            .GroupBy(t => t.Year)
            .Select(g => new YearCountDto { Year = g.Key, Count = g.Sum(x => x.PaperCount) })
            .OrderBy(x => x.Year)
            .ToList();

        var keywordGroups = trends
            .GroupBy(t => t.Keyword)
            .Select(g => new { Keyword = g.Key, Count = g.Sum(x => x.PaperCount) })
            .OrderByDescending(x => x.Count)
            .ToList();

        var topKeyword = keywordGroups.FirstOrDefault();

        return new DashboardResponse
        {
            TotalPapers = trends.Sum(t => t.PaperCount),
            PapersByYear = papersByYear,
            TopKeywords = keywordGroups.Take(10)
                .Select(x => new KeywordCountDto { Keyword = x.Keyword, Count = x.Count })
                .ToList(),
            TopKeyword = topKeyword?.Keyword,
            TopKeywordCount = topKeyword?.Count ?? 0,
            TotalKeywords = keywordGroups.Count,
            YearFrom = papersByYear.Count == 0 ? 0 : papersByYear.Min(y => y.Year),
            YearTo = papersByYear.Count == 0 ? 0 : papersByYear.Max(y => y.Year),
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<IReadOnlyList<ReportResponse>> GetReportsAsync(CancellationToken cancellationToken = default)
    {
        var reports = await _unitOfWork.DashboardReports.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ReportResponse>>(reports);
    }

    public async Task<ReportResponse> GenerateDailyReportAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await _unitOfWork.DashboardReports.GetByDateAsync(today, cancellationToken);

        var trends = await _unitOfWork.PublicationTrends.GetAllAsync(cancellationToken);
        var totalPapers = trends.Sum(t => t.PaperCount);
        var topKeyword = trends
            .GroupBy(t => t.Keyword)
            .Select(g => new { Keyword = g.Key, Count = g.Sum(x => x.PaperCount) })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        if (existing is not null)
        {
            existing.TotalPapers = totalPapers;
            existing.TopKeyword = topKeyword?.Keyword ?? "N/A";
            existing.GeneratedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReportResponse>(existing);
        }

        var report = new DashboardReport
        {
            Id = Guid.NewGuid(),
            ReportDate = today,
            TotalPapers = totalPapers,
            TopKeyword = topKeyword?.Keyword ?? "N/A",
            GeneratedAt = DateTime.UtcNow
        };

        await _unitOfWork.DashboardReports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReportResponse>(report);
    }
}
