using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Configurations;

public class ResearchPaperConfiguration : IEntityTypeConfiguration<ResearchPaper>
{
    public void Configure(EntityTypeBuilder<ResearchPaper> builder)
    {
        builder.ToTable("ResearchPapers");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Abstract).HasMaxLength(4000);
        builder.Property(p => p.Doi).HasMaxLength(200);
        builder.HasIndex(p => p.Doi).IsUnique();
        builder.HasOne(p => p.Journal).WithMany(j => j.Papers).HasForeignKey(p => p.JournalId);
    }
}
