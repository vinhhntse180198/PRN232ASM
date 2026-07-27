namespace PRN232ASM.PaperService.Application.Interfaces;

public interface IRecommendationClient
{
    Task<IReadOnlyList<RecommendationResult>> GetRecommendationsAsync(
        RecommendationQuery query,
        CancellationToken cancellationToken = default);
}

public record RecommendationQuery(
    Guid SourcePaperId,
    int Limit,
    IReadOnlyList<string> SourceKeywords,
    IReadOnlyList<string> SourceTopics,
    string SourceJournalName,
    int SourceYear,
    IReadOnlyList<RecommendationCandidate> Candidates);

public record RecommendationCandidate(
    Guid PaperId,
    string Title,
    int PublicationYear,
    string JournalName,
    IReadOnlyList<string> Keywords,
    IReadOnlyList<string> Topics);

public record RecommendationResult(
    Guid PaperId,
    double Score,
    string Reason);
