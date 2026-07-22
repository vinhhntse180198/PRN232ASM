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

        return new DashboardResponse
        {
            TotalPapers = trends.Sum(t => t.PaperCount),
            GeneratedAt = DateTime.UtcNow,
            PapersByYear = trends
                .GroupBy(t => t.Year)
                .Select(g => new YearCountDto { Year = g.Key, Count = g.Sum(x => x.PaperCount) })
                .OrderBy(x => x.Year)
                .ToList(),
            TopKeywords = trends
                .GroupBy(t => t.Keyword)
                .Select(g => new KeywordCountDto { Keyword = g.Key, Count = g.Sum(x => x.PaperCount) })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList()
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
        if (existing is not null)
        {
            return _mapper.Map<ReportResponse>(existing);
        }

        var trends = await _unitOfWork.PublicationTrends.GetAllAsync(cancellationToken);
        var topKeyword = trends
            .GroupBy(t => t.Keyword)
            .Select(g => new { Keyword = g.Key, Count = g.Sum(x => x.PaperCount) })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        var report = new DashboardReport
        {
            Id = Guid.NewGuid(),
            ReportDate = today,
            TotalPapers = trends.Sum(t => t.PaperCount),
            TopKeyword = topKeyword?.Keyword ?? "N/A",
            GeneratedAt = DateTime.UtcNow
        };

        await _unitOfWork.DashboardReports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReportResponse>(report);
    }
}
