using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("Bookmarks");
        builder.HasKey(b => new { b.UserId, b.PaperId });
        builder.HasOne(b => b.Paper).WithMany(p => p.Bookmarks).HasForeignKey(b => b.PaperId);
    }
}

public class PaperAuthorConfiguration : IEntityTypeConfiguration<PaperAuthor>
{
    public void Configure(EntityTypeBuilder<PaperAuthor> builder)
    {
        builder.ToTable("PaperAuthors");
        builder.HasKey(pa => new { pa.PaperId, pa.AuthorId });
        builder.HasOne(pa => pa.Paper).WithMany(p => p.PaperAuthors).HasForeignKey(pa => pa.PaperId);
        builder.HasOne(pa => pa.Author).WithMany(a => a.PaperAuthors).HasForeignKey(pa => pa.AuthorId);
    }
}

public class PaperKeywordConfiguration : IEntityTypeConfiguration<PaperKeyword>
{
    public void Configure(EntityTypeBuilder<PaperKeyword> builder)
    {
        builder.ToTable("PaperKeywords");
        builder.HasKey(pk => new { pk.PaperId, pk.KeywordId });
        builder.HasOne(pk => pk.Paper).WithMany(p => p.PaperKeywords).HasForeignKey(pk => pk.PaperId);
        builder.HasOne(pk => pk.Keyword).WithMany(k => k.PaperKeywords).HasForeignKey(pk => pk.KeywordId);
    }
}

public class PaperTopicConfiguration : IEntityTypeConfiguration<PaperTopic>
{
    public void Configure(EntityTypeBuilder<PaperTopic> builder)
    {
        builder.ToTable("PaperTopics");
        builder.HasKey(pt => new { pt.PaperId, pt.TopicId });
        builder.HasOne(pt => pt.Paper).WithMany(p => p.PaperTopics).HasForeignKey(pt => pt.PaperId);
        builder.HasOne(pt => pt.Topic).WithMany(t => t.PaperTopics).HasForeignKey(pt => pt.TopicId);
    }
}
