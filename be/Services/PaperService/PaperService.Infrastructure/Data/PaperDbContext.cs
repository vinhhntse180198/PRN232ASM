using PaperService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PaperService.Infrastructure.Data;

public class PaperDbContext : DbContext
{
    public const string Schema = "paper";

    public PaperDbContext(DbContextOptions<PaperDbContext> options) : base(options) { }

    public DbSet<ResearchPaper> ResearchPapers => Set<ResearchPaper>();
    public DbSet<Journal> Journals => Set<Journal>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Keyword> Keywords => Set<Keyword>();
    public DbSet<PaperAuthor> PaperAuthors => Set<PaperAuthor>();
    public DbSet<PaperKeyword> PaperKeywords => Set<PaperKeyword>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsNpgsql())
            modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.ToTable("journals");
            entity.HasKey(j => j.Id);
            entity.Property(j => j.Name).HasMaxLength(500).IsRequired();
            entity.HasIndex(j => j.Name);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(255).IsRequired();
            entity.HasIndex(a => a.Name);
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.ToTable("keywords");
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Name).HasMaxLength(255).IsRequired();
            entity.HasIndex(k => k.Name).IsUnique();
        });

        modelBuilder.Entity<ResearchPaper>(entity =>
        {
            entity.ToTable("research_papers");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired();
            entity.Property(p => p.Doi).HasMaxLength(255);
            entity.HasIndex(p => p.Doi).IsUnique();
            entity.HasIndex(p => p.PublishedYear);
            entity.Property(p => p.IsOpenAccess).HasDefaultValue(false);
            entity.HasOne(p => p.Journal).WithMany(j => j.Papers).HasForeignKey(p => p.JournalId);
        });

        modelBuilder.Entity<PaperAuthor>(entity =>
        {
            entity.ToTable("paper_authors");
            entity.HasKey(pa => new { pa.PaperId, pa.AuthorId });
            entity.HasOne(pa => pa.Paper).WithMany(p => p.PaperAuthors).HasForeignKey(pa => pa.PaperId);
            entity.HasOne(pa => pa.Author).WithMany(a => a.PaperAuthors).HasForeignKey(pa => pa.AuthorId);
        });

        modelBuilder.Entity<PaperKeyword>(entity =>
        {
            entity.ToTable("paper_keywords");
            entity.HasKey(pk => new { pk.PaperId, pk.KeywordId });
            entity.HasOne(pk => pk.Paper).WithMany(p => p.PaperKeywords).HasForeignKey(pk => pk.PaperId);
            entity.HasOne(pk => pk.Keyword).WithMany(k => k.PaperKeywords).HasForeignKey(pk => pk.KeywordId);
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.ToTable("bookmarks");
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => new { b.UserId, b.PaperId }).IsUnique();
            entity.HasOne(b => b.Paper).WithMany(p => p.Bookmarks).HasForeignKey(b => b.PaperId);
        });
    }
}
