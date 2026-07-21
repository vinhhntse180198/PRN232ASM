using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Infrastructure.Persistence.Configurations;

public class PublicationTrendConfiguration : IEntityTypeConfiguration<PublicationTrend>
{
    public void Configure(EntityTypeBuilder<PublicationTrend> builder)
    {
        builder.ToTable("PublicationTrends");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Keyword).HasMaxLength(100).IsRequired();
        builder.HasIndex(t => new { t.Keyword, t.Year, t.TopicId }).IsUnique();
    }
}
