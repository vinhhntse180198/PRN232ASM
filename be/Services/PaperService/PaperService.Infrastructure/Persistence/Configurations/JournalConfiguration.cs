using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Configurations;

public class JournalConfiguration : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.ToTable("Journals");
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Name).HasMaxLength(300).IsRequired();
        builder.HasIndex(j => j.Name).IsUnique();
    }
}
