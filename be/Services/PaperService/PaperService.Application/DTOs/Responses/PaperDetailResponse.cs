namespace PaperService.Application.DTOs.Responses;

public class PaperListItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public short? PublishedYear { get; set; }
    public int CitationCount { get; set; }
    public string? JournalName { get; set; }
    public IReadOnlyList<string> Authors { get; set; } = [];
    public IReadOnlyList<string> Keywords { get; set; } = [];
    public bool IsBookmarked { get; set; }
}

public class PaperDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public short? PublishedYear { get; set; }
    public DateOnly? PublishedDate { get; set; }
    public int CitationCount { get; set; }
    public string? Url { get; set; }
    public JournalSummaryResponse? Journal { get; set; }
    public IReadOnlyList<AuthorSummaryResponse> Authors { get; set; } = [];
    public IReadOnlyList<string> Keywords { get; set; } = [];
    public IReadOnlyList<TopicSummaryResponse> Topics { get; set; } = [];
    public bool IsBookmarked { get; set; }
}

public class JournalSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issn { get; set; }
    public string? Publisher { get; set; }
}

public class AuthorSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Affiliation { get; set; }
    public short Order { get; set; }
}

public class TopicSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class JournalListItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issn { get; set; }
    public string? Publisher { get; set; }
    public int PaperCount { get; set; }
}

public class KeywordListItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PaperCount { get; set; }
}

public class TopicListItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PaperCount { get; set; }
}

public class BookmarkResponse
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? JournalName { get; set; }
    public short? PublishedYear { get; set; }
    public DateTime CreatedAt { get; set; }
}
