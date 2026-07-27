namespace PRN232ASM.PaperService.Application.Interfaces;

public interface IPricingClient
{
    Task<ImpactScoreResult> ComputeImpactScoreAsync(ImpactScoreQuery query, CancellationToken cancellationToken = default);
}

public record ImpactScoreQuery(
    Guid PaperId,
    string Title,
    int PublicationYear,
    int CitationCount,
    int KeywordCount,
    int TopicCount,
    int AuthorCount,
    string JournalName);

public record ImpactScoreResult(
    Guid PaperId,
    double Score,
    string CurrencyLabel,
    string Tier,
    string Explanation);

public interface IInferenceClient
{
    Task<InsightResult> InferInsightsAsync(InsightQuery query, CancellationToken cancellationToken = default);
}

public record InsightQuery(
    Guid PaperId,
    string Title,
    string Abstract,
    IReadOnlyList<string> ExistingKeywords,
    IReadOnlyList<string> ExistingTopics);

public record InsightResult(
    Guid PaperId,
    IReadOnlyList<string> SuggestedKeywords,
    IReadOnlyList<string> SuggestedTopics,
    double Confidence,
    string Summary);

public interface IInventoryClient
{
    Task<JournalCapacityResult> GetCapacityAsync(JournalCapacityQuery query, CancellationToken cancellationToken = default);
}

public record JournalCapacityQuery(
    Guid JournalId,
    string JournalName,
    int Year,
    int PapersInYear,
    int TotalPapers);

public record JournalCapacityResult(
    Guid JournalId,
    int Year,
    int Capacity,
    int Used,
    int Remaining,
    string Status,
    string Note);

public interface IUserProfileClient
{
    Task<ReadingProfileResult> BuildReadingProfileAsync(ReadingProfileQuery query, CancellationToken cancellationToken = default);
}

public record ReadingProfileQuery(
    Guid UserId,
    IReadOnlyList<BookmarkedPaperSignal> Bookmarks,
    IReadOnlyList<string> FollowedKeywords,
    IReadOnlyList<string> FollowedTopics,
    IReadOnlyList<string> FollowedJournals);

public record BookmarkedPaperSignal(
    Guid PaperId,
    string Title,
    IReadOnlyList<string> Keywords,
    IReadOnlyList<string> Topics,
    string JournalName,
    int PublicationYear);

public record ReadingProfileResult(
    Guid UserId,
    int BookmarkCount,
    IReadOnlyList<InterestItemResult> TopKeywords,
    IReadOnlyList<InterestItemResult> TopTopics,
    IReadOnlyList<InterestItemResult> TopJournals,
    string PersonaLabel,
    string Summary);

public record InterestItemResult(string Name, int Weight);
