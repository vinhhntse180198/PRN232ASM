using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Configurations;

public class ResearchTopicConfiguration : IEntityTypeConfiguration<ResearchTopic>
{
    public void Configure(EntityTypeBuilder<ResearchTopic> builder)
    {
        builder.ToTable("ResearchTopics");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();
    }
}
