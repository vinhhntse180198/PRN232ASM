using Grpc.Core;
using PRN232ASM.Recommendation.Grpc;

namespace PRN232ASM.RecommendationService.Services;

public class RecommendationGrpcService : PaperRecommender.PaperRecommenderBase
{
    private readonly ILogger<RecommendationGrpcService> _logger;

    public RecommendationGrpcService(ILogger<RecommendationGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<RecommendReply> GetRecommendations(
        RecommendRequest request,
        ServerCallContext context)
    {
        var limit = request.Limit <= 0 ? 5 : Math.Min(request.Limit, 20);
        var sourceKeywords = request.SourceKeywords
            .Select(Normalize)
            .Where(x => x.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var sourceTopics = request.SourceTopics
            .Select(Normalize)
            .Where(x => x.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var sourceJournal = Normalize(request.SourceJournalName);

        var scored = new List<(RecommendItem Item, double Raw)>();

        foreach (var candidate in request.Candidates)
        {
            if (string.Equals(candidate.PaperId, request.SourcePaperId, StringComparison.OrdinalIgnoreCase))
                continue;

            var sharedKeywords = candidate.Keywords
                .Select(Normalize)
                .Where(k => k.Length > 0 && sourceKeywords.Contains(k))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var sharedTopics = candidate.Topics
                .Select(Normalize)
                .Where(t => t.Length > 0 && sourceTopics.Contains(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var sameJournal = !string.IsNullOrEmpty(sourceJournal)
                && string.Equals(Normalize(candidate.JournalName), sourceJournal, StringComparison.OrdinalIgnoreCase);

            var yearClose = request.SourceYear > 0
                && candidate.PublicationYear > 0
                && Math.Abs(candidate.PublicationYear - request.SourceYear) <= 2;

            var raw = sharedKeywords.Count * 2
                + sharedTopics.Count * 3
                + (sameJournal ? 1 : 0)
                + (yearClose ? 1 : 0);

            if (raw <= 0)
                continue;

            var reasons = new List<string>();
            if (sharedKeywords.Count > 0)
                reasons.Add($"Shared keywords: {string.Join(", ", sharedKeywords.Take(3))}");
            if (sharedTopics.Count > 0)
                reasons.Add($"Shared topics: {string.Join(", ", sharedTopics.Take(2))}");
            if (sameJournal)
                reasons.Add("Same journal");
            if (yearClose)
                reasons.Add("Similar publication year");

            scored.Add((
                new RecommendItem
                {
                    PaperId = candidate.PaperId,
                    Score = raw,
                    Reason = string.Join("; ", reasons)
                },
                raw));
        }

        var maxRaw = scored.Count == 0 ? 1 : scored.Max(x => x.Raw);
        var reply = new RecommendReply();
        foreach (var item in scored
                     .OrderByDescending(x => x.Raw)
                     .ThenBy(x => x.Item.PaperId)
                     .Take(limit))
        {
            item.Item.Score = Math.Round(item.Raw / maxRaw, 4);
            reply.Items.Add(item.Item);
        }

        _logger.LogInformation(
            "gRPC GetRecommendations for {PaperId}: {CandidateCount} candidates -> {ResultCount} results",
            request.SourcePaperId,
            request.Candidates.Count,
            reply.Items.Count);

        return Task.FromResult(reply);
    }

    private static string Normalize(string? value)
        => (value ?? string.Empty).Trim().ToLowerInvariant();
}
