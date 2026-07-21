using PRN232ASM.TrendService.Application.Interfaces.Repositories;

namespace PRN232ASM.TrendService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPublicationTrendRepository PublicationTrends { get; }
    IDashboardReportRepository DashboardReports { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
