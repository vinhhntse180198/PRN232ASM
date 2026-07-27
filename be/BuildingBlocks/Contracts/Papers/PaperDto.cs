namespace PRN232ASM.BuildingBlocks.Contracts.Papers;

public record PaperDto(
    Guid Id,
    string Title,
    string? Abstract,
    int? PublicationYear,
    string? Doi,
    int CitationCount,
    Guid? JournalId,
    IReadOnlyList<Guid> TopicIds,
    IReadOnlyList<Guid> KeywordIds,
    IReadOnlyList<string> AuthorNames)
{
    public string? Url { get; init; }
    public string? PdfUrl { get; init; }
}
