namespace PaperService.Application.DTOs;

public class CreatePaperRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? JournalName { get; set; }
    public short? PublishedYear { get; set; }
    public DateOnly? PublishedDate { get; set; }
    public string? Url { get; set; }
    public string? ExternalId { get; set; }
    public int? CitationCount { get; set; }
    public bool? IsOpenAccess { get; set; }
    public List<string>? AuthorNames { get; set; }
    public List<string>? Keywords { get; set; }
}
