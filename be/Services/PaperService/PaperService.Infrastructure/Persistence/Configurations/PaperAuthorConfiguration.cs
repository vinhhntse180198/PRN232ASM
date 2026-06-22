using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class PaperAuthorConfiguration : IEntityTypeConfiguration<PaperAuthor>
{
    public void Configure(EntityTypeBuilder<PaperAuthor> builder)
    {
        builder.ToTable("paper_authors");
        builder.HasKey(pa => new { pa.PaperId, pa.AuthorId });
        builder.HasOne(pa => pa.Paper).WithMany(p => p.PaperAuthors).HasForeignKey(pa => pa.PaperId);
        builder.HasOne(pa => pa.Author).WithMany(a => a.PaperAuthors).HasForeignKey(pa => pa.AuthorId);
    }
}
