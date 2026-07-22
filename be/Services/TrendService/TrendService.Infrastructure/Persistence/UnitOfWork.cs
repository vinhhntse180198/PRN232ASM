using PRN232ASM.TrendService.Application.Interfaces;
using PRN232ASM.TrendService.Application.Interfaces.Repositories;
using PRN232ASM.TrendService.Infrastructure.Persistence.Repositories;

namespace PRN232ASM.TrendService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TrendServiceDbContext _context;

    public UnitOfWork(TrendServiceDbContext context)
    {
        _context = context;
        PublicationTrends = new PublicationTrendRepository(context);
        DashboardReports = new DashboardReportRepository(context);
    }

    public IPublicationTrendRepository PublicationTrends { get; }
    public IDashboardReportRepository DashboardReports { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
