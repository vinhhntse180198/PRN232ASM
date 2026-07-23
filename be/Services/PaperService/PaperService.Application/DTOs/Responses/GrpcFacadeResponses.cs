namespace PRN232ASM.PaperService.Application.DTOs.Responses;

public class PaperImpactScoreResponse
{
    public Guid PaperId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Score { get; set; }
    public string CurrencyLabel { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

public class PaperInsightResponse
{
    public Guid PaperId { get; set; }
    public IReadOnlyList<string> SuggestedKeywords { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> SuggestedTopics { get; set; } = Array.Empty<string>();
    public double Confidence { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public class JournalCapacityResponse
{
    public Guid JournalId { get; set; }
    public string JournalName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Capacity { get; set; }
    public int Used { get; set; }
    public int Remaining { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public class ReadingProfileResponse
{
    public Guid UserId { get; set; }
    public int BookmarkCount { get; set; }
    public IReadOnlyList<InterestItemResponse> TopKeywords { get; set; } = Array.Empty<InterestItemResponse>();
    public IReadOnlyList<InterestItemResponse> TopTopics { get; set; } = Array.Empty<InterestItemResponse>();
    public IReadOnlyList<InterestItemResponse> TopJournals { get; set; } = Array.Empty<InterestItemResponse>();
    public string PersonaLabel { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class InterestItemResponse
{
    public string Name { get; set; } = string.Empty;
    public int Weight { get; set; }
}
