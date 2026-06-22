using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class PaperKeywordConfiguration : IEntityTypeConfiguration<PaperKeyword>
{
    public void Configure(EntityTypeBuilder<PaperKeyword> builder)
    {
        builder.ToTable("paper_keywords");
        builder.HasKey(pk => new { pk.PaperId, pk.KeywordId });
        builder.HasOne(pk => pk.Paper).WithMany(p => p.PaperKeywords).HasForeignKey(pk => pk.PaperId);
        builder.HasOne(pk => pk.Keyword).WithMany(k => k.PaperKeywords).HasForeignKey(pk => pk.KeywordId);
    }
}
