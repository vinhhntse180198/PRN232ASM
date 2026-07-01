namespace SyncService.Application.DTOs;

public class OpenAlexPaperResult
{
    public string? OpenAlexId { get; set; }
    public string? OpenAlexUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? JournalName { get; set; }
    public short? PublishedYear { get; set; }
    public int CitationCount { get; set; }
    public string? Url { get; set; }
    public string? PdfUrl { get; set; }
    public string? OaUrl { get; set; }
    public string? ReadUrl { get; set; }
    public bool IsOpenAccess { get; set; }
    public bool CanReadInApp { get; set; }
    public bool IsPaywalled { get; set; }
    public bool HasPdf { get; set; }
    public IReadOnlyList<string> Authors { get; set; } = [];
    public IReadOnlyList<string> Keywords { get; set; } = [];
}

public class OpenAlexSearchResponse
{
    public IReadOnlyList<OpenAlexPaperResult> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
