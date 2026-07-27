namespace PRN232ASM.PaperService.Application.DTOs.Requests;

public record CreatePaperRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Abstract { get; init; }
    public string? Doi { get; init; }
    public int PublicationYear { get; init; }
    public int CitationCount { get; init; }
    public string JournalName { get; init; } = string.Empty;
    public IReadOnlyList<string> Authors { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Keywords { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Topics { get; init; } = Array.Empty<string>();
    public string? Url { get; init; }
    public string? PdfUrl { get; init; }
}
