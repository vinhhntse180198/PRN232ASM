using Microsoft.EntityFrameworkCore;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Persistence.Configurations;

namespace PaperService.Infrastructure.Persistence;

public class PaperServiceDbContext : DbContext
{
    public PaperServiceDbContext(DbContextOptions<PaperServiceDbContext> options) : base(options) { }

    public DbSet<ResearchPaper> ResearchPapers => Set<ResearchPaper>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Journal> Journals => Set<Journal>();
    public DbSet<Keyword> Keywords => Set<Keyword>();
    public DbSet<ResearchTopic> ResearchTopics => Set<ResearchTopic>();
    public DbSet<PaperAuthor> PaperAuthors => Set<PaperAuthor>();
    public DbSet<PaperKeyword> PaperKeywords => Set<PaperKeyword>();
    public DbSet<PaperTopic> PaperTopics => Set<PaperTopic>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ResearchPaperConfiguration());
        modelBuilder.ApplyConfiguration(new AuthorConfiguration());
        modelBuilder.ApplyConfiguration(new JournalConfiguration());
        modelBuilder.ApplyConfiguration(new KeywordConfiguration());
        modelBuilder.ApplyConfiguration(new ResearchTopicConfiguration());
        modelBuilder.ApplyConfiguration(new BookmarkConfiguration());
        modelBuilder.ApplyConfiguration(new PaperAuthorConfiguration());
        modelBuilder.ApplyConfiguration(new PaperKeywordConfiguration());
        modelBuilder.ApplyConfiguration(new PaperTopicConfiguration());
    }
}
