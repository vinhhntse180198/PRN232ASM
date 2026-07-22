using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Configurations;

public class KeywordConfiguration : IEntityTypeConfiguration<Keyword>
{
    public void Configure(EntityTypeBuilder<Keyword> builder)
    {
        builder.ToTable("Keywords");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(k => k.Name).IsUnique();
    }
}
