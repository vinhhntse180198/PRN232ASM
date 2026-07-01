using TrendService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TrendService.Infrastructure.Data;

public class TrendDbContext : DbContext
{
    public const string Schema = "trend";

    public TrendDbContext(DbContextOptions<TrendDbContext> options) : base(options) { }

    public DbSet<PublicationTrend> PublicationTrends => Set<PublicationTrend>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsNpgsql())
            modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<PublicationTrend>(entity =>
        {
            entity.ToTable("publication_trends");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.KeywordName).HasMaxLength(255);
            entity.Property(t => t.GrowthRate).HasPrecision(8, 4);
            entity.HasIndex(t => new { t.KeywordName, t.Year });
        });
    }
}
