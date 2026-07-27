using Microsoft.EntityFrameworkCore;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Infrastructure.Persistence;

public class TrendServiceDbContext : DbContext
{
    public TrendServiceDbContext(DbContextOptions<TrendServiceDbContext> options) : base(options)
    {
    }

    public DbSet<PublicationTrend> PublicationTrends => Set<PublicationTrend>();
    public DbSet<DashboardReport> DashboardReports => Set<DashboardReport>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrendServiceDbContext).Assembly);
        modelBuilder.ConfigureOutboxMessages();
        base.OnModelCreating(modelBuilder);
    }
}
