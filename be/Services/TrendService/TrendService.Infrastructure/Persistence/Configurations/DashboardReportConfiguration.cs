using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Infrastructure.Persistence.Configurations;

public class DashboardReportConfiguration : IEntityTypeConfiguration<DashboardReport>
{
    public void Configure(EntityTypeBuilder<DashboardReport> builder)
    {
        builder.ToTable("DashboardReports");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.TopKeyword).HasMaxLength(100).IsRequired();
        builder.HasIndex(r => r.ReportDate).IsUnique();
    }
}
