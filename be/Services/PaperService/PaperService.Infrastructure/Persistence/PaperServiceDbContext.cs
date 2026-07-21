using Microsoft.EntityFrameworkCore;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence;

public class PaperServiceDbContext : DbContext
{
    public PaperServiceDbContext(DbContextOptions<PaperServiceDbContext> options) : base(options)
    {
    }

    public DbSet<ResearchPaper> ResearchPapers => Set<ResearchPaper>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Journal> Journals => Set<Journal>();
    public DbSet<Keyword> Keywords => Set<Keyword>();
    public DbSet<ResearchTopic> ResearchTopics => Set<ResearchTopic>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<PaperAuthor> PaperAuthors => Set<PaperAuthor>();
    public DbSet<PaperKeyword> PaperKeywords => Set<PaperKeyword>();
    public DbSet<PaperTopic> PaperTopics => Set<PaperTopic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaperServiceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
