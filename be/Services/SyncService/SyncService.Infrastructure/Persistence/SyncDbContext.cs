using Microsoft.EntityFrameworkCore;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using SyncService.Domain.Entities;

namespace SyncService.Infrastructure.Persistence;

public class SyncDbContext : DbContext
{
    public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options) { }

    public DbSet<DataSource> DataSources => Set<DataSource>();
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.ToTable("DataSources");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.BaseUrl).HasMaxLength(500).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<SyncLog>(entity =>
        {
            entity.ToTable("SyncLogs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasOne(x => x.DataSource)
                .WithMany(x => x.SyncLogs)
                .HasForeignKey(x => x.DataSourceId);
        });

        modelBuilder.ConfigureOutboxMessages();
    }
}
