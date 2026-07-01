namespace PaperService.Domain.Entities;

public class ResearchPaper
{
    public Guid Id { get; set; }
    public Guid? JournalId { get; set; }
    public Guid? DataSourceId { get; set; }
    public string? ExternalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public short? PublishedYear { get; set; }
    public DateOnly? PublishedDate { get; set; }
    public int CitationCount { get; set; }
    public bool IsOpenAccess { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Journal? Journal { get; set; }
    public ICollection<PaperAuthor> PaperAuthors { get; set; } = [];
    public ICollection<PaperKeyword> PaperKeywords { get; set; } = [];
    public ICollection<Bookmark> Bookmarks { get; set; } = [];
}
