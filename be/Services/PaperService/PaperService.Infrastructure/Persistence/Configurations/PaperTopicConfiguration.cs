using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class PaperTopicConfiguration : IEntityTypeConfiguration<PaperTopic>
{
    public void Configure(EntityTypeBuilder<PaperTopic> builder)
    {
        builder.ToTable("paper_topics");
        builder.HasKey(pt => new { pt.PaperId, pt.TopicId });
        builder.HasOne(pt => pt.Paper).WithMany(p => p.PaperTopics).HasForeignKey(pt => pt.PaperId);
        builder.HasOne(pt => pt.Topic).WithMany(t => t.PaperTopics).HasForeignKey(pt => pt.TopicId);
    }
}
