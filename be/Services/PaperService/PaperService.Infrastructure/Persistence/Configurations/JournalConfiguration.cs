using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class JournalConfiguration : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.ToTable("journals");
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Name).IsRequired().HasMaxLength(500);
        builder.Property(j => j.Issn).HasMaxLength(20);
    }
}
