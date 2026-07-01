namespace PaperService.Application.DTOs;

public class PaperResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public Guid? JournalId { get; set; }
    public string? JournalName { get; set; }
    public short? PublishedYear { get; set; }
    public DateTime? PublishedDate { get; set; }
    public int CitationCount { get; set; }
    public bool IsOpenAccess { get; set; }
    public string? Url { get; set; }
    public string? ExternalId { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<AuthorResponse> Authors { get; set; } = [];
    public IReadOnlyList<string> Keywords { get; set; } = [];
    public bool IsBookmarked { get; set; }
}
