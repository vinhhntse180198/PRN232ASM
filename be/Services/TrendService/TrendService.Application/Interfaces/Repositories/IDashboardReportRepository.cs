using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Application.Interfaces.Repositories;

public interface IDashboardReportRepository
{
    Task<IReadOnlyList<DashboardReport>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DashboardReport?> GetByDateAsync(DateOnly reportDate, CancellationToken cancellationToken = default);
    Task AddAsync(DashboardReport report, CancellationToken cancellationToken = default);
}
