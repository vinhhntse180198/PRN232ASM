namespace PRN232ASM.PaperService.Domain.Entities;

public class ResearchPaper
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Doi { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public int CitationCount { get; set; }
    public Guid JournalId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Url { get; set; }
    public string? PdfUrl { get; set; }

    public Journal Journal { get; set; } = null!;
    public ICollection<PaperAuthor> PaperAuthors { get; set; } = new List<PaperAuthor>();
    public ICollection<PaperKeyword> PaperKeywords { get; set; } = new List<PaperKeyword>();
    public ICollection<PaperTopic> PaperTopics { get; set; } = new List<PaperTopic>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
