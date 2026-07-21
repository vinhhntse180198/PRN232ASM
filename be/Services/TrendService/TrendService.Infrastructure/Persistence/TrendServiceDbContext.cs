using Microsoft.EntityFrameworkCore;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Infrastructure.Persistence;

public class TrendServiceDbContext : DbContext
{
    public TrendServiceDbContext(DbContextOptions<TrendServiceDbContext> options) : base(options)
    {
    }

    public DbSet<PublicationTrend> PublicationTrends => Set<PublicationTrend>();
    public DbSet<DashboardReport> DashboardReports => Set<DashboardReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrendServiceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
