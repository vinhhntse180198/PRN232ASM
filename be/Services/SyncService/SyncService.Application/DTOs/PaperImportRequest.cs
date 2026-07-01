namespace SyncService.Application.DTOs;

public class PaperImportRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? ExternalId { get; set; }
    public string? JournalName { get; set; }
    public short? PublishedYear { get; set; }
    public DateOnly? PublishedDate { get; set; }
    public string? Url { get; set; }
    public int CitationCount { get; set; }
    public List<string>? AuthorNames { get; set; }
    public List<string>? Keywords { get; set; }
}
