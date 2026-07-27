namespace PRN232ASM.PaperService.Application.DTOs.Requests;

public class SearchPaperRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
    public string? Author { get; set; }
    public string? Journal { get; set; }
    public Guid? TopicId { get; set; }
}

public class ImportPaperRequest
{
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public int? PublicationYear { get; set; }
    public string? Doi { get; set; }
    public int CitationCount { get; set; }
    public string? JournalName { get; set; }
    public IReadOnlyList<string> AuthorNames { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Keywords { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Topics { get; set; } = Array.Empty<string>();
}
