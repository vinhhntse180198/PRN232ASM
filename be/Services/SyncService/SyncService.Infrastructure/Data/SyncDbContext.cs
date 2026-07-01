using SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SyncService.Infrastructure.Data;

public class SyncDbContext : DbContext
{
    public const string Schema = "sync";

    public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options) { }

    public DbSet<DataSource> DataSources => Set<DataSource>();
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsNpgsql())
            modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.ToTable("data_sources");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(d => d.Name).IsUnique();
            entity.Property(d => d.BaseUrl).IsRequired();
        });

        modelBuilder.Entity<SyncLog>(entity =>
        {
            entity.ToTable("sync_logs");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Status).HasMaxLength(20).IsRequired();
            entity.HasOne(l => l.DataSource).WithMany().HasForeignKey(l => l.DataSourceId);
        });
    }
}
