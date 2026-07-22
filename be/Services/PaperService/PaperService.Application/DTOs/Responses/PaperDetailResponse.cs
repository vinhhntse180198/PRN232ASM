namespace PRN232ASM.PaperService.Application.DTOs.Responses;

public class PaperSummaryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Doi { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public int CitationCount { get; set; }
    public string JournalName { get; set; } = string.Empty;
    public IReadOnlyList<string> Authors { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Keywords { get; set; } = Array.Empty<string>();
}

public class PaperDetailResponse : PaperSummaryResponse
{
    public IReadOnlyList<string> Topics { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; }
}

public class AuthorResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Affiliation { get; set; }
}

public class JournalResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issn { get; set; }
    public string? Publisher { get; set; }
}

public class KeywordResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TopicResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class BookmarkResponse
{
    public Guid UserId { get; set; }
    public Guid PaperId { get; set; }
    public string PaperTitle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
