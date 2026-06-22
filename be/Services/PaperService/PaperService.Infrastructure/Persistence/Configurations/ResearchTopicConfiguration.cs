using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class ResearchTopicConfiguration : IEntityTypeConfiguration<ResearchTopic>
{
    public void Configure(EntityTypeBuilder<ResearchTopic> builder)
    {
        builder.ToTable("research_topics");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(t => t.Name).IsUnique();
    }
}
