namespace PRN232ASM.PaperService.Application.DTOs.Requests;

public class SearchPaperRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
    public string? Author { get; set; }
    public string? Journal { get; set; }
}

public class CreatePaperRequest
{
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Doi { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public int CitationCount { get; set; }
    public string JournalName { get; set; } = string.Empty;
    public IReadOnlyList<string> Authors { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Keywords { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Topics { get; set; } = Array.Empty<string>();
}

public class BookmarkRequest
{
    public Guid PaperId { get; set; }
}
