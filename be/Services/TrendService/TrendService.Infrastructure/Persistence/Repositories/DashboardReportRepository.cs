using Microsoft.EntityFrameworkCore;
using PRN232ASM.TrendService.Application.Interfaces.Repositories;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Infrastructure.Persistence.Repositories;

public class DashboardReportRepository : IDashboardReportRepository
{
    private readonly TrendServiceDbContext _context;

    public DashboardReportRepository(TrendServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DashboardReport report, CancellationToken cancellationToken = default)
    {
        await _context.DashboardReports.AddAsync(report, cancellationToken);
    }

    public async Task<IReadOnlyList<DashboardReport>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DashboardReports
            .AsNoTracking()
            .OrderByDescending(r => r.ReportDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<DashboardReport?> GetByDateAsync(DateOnly reportDate, CancellationToken cancellationToken = default)
    {
        return await _context.DashboardReports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ReportDate == reportDate, cancellationToken);
    }
}
