using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class ResearchPaperConfiguration : IEntityTypeConfiguration<ResearchPaper>
{
    public void Configure(EntityTypeBuilder<ResearchPaper> builder)
    {
        builder.ToTable("research_papers");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Doi).HasMaxLength(255);
        builder.HasIndex(p => p.Doi).IsUnique();
        builder.HasOne(p => p.Journal).WithMany(j => j.Papers).HasForeignKey(p => p.JournalId);
    }
}
