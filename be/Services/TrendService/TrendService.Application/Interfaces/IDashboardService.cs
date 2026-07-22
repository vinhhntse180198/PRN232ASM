using PRN232ASM.TrendService.Application.DTOs;

namespace PRN232ASM.TrendService.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportResponse>> GetReportsAsync(CancellationToken cancellationToken = default);
    Task<ReportResponse> GenerateDailyReportAsync(CancellationToken cancellationToken = default);
}
