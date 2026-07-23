using Grpc.Core;
using PRN232ASM.UserProfile.Grpc;

namespace PRN232ASM.UserProfileService.Services;

public class UserProfileGrpcService : ReaderProfiler.ReaderProfilerBase
{
    private readonly ILogger<UserProfileGrpcService> _logger;

    public UserProfileGrpcService(ILogger<UserProfileGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<ReadingProfileReply> BuildReadingProfile(
        ReadingProfileRequest request,
        ServerCallContext context)
    {
        var keywordWeights = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var topicWeights = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var journalWeights = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var bm in request.Bookmarks)
        {
            foreach (var kw in bm.Keywords)
                Bump(keywordWeights, kw, 2);
            foreach (var topic in bm.Topics)
                Bump(topicWeights, topic, 3);
            Bump(journalWeights, bm.JournalName, 2);
        }

        foreach (var kw in request.FollowedKeywords)
            Bump(keywordWeights, kw, 4);
        foreach (var topic in request.FollowedTopics)
            Bump(topicWeights, topic, 5);
        foreach (var journal in request.FollowedJournals)
            Bump(journalWeights, journal, 4);

        var topKeywords = Top(keywordWeights, 8);
        var topTopics = Top(topicWeights, 6);
        var topJournals = Top(journalWeights, 5);

        var persona = topTopics.Count > 0
            ? $"{topTopics[0].Name} Explorer"
            : topKeywords.Count > 0
                ? $"{topKeywords[0].Name} Reader"
                : "Curious Browser";

        var reply = new ReadingProfileReply
        {
            UserId = request.UserId,
            BookmarkCount = request.Bookmarks.Count,
            PersonaLabel = persona,
            Summary = request.Bookmarks.Count == 0 && request.FollowedKeywords.Count == 0
                ? "No reading signals yet. Bookmark papers or follow topics to build a profile."
                : $"Profile from {request.Bookmarks.Count} bookmarks + follows → persona '{persona}'."
        };
        reply.TopKeywords.AddRange(topKeywords);
        reply.TopTopics.AddRange(topTopics);
        reply.TopJournals.AddRange(topJournals);

        _logger.LogInformation(
            "Reading profile for {UserId}: bookmarks={Count}, persona={Persona}",
            request.UserId,
            request.Bookmarks.Count,
            persona);

        return Task.FromResult(reply);
    }

    private static void Bump(Dictionary<string, int> map, string? key, int weight)
    {
        key = (key ?? string.Empty).Trim();
        if (key.Length == 0) return;
        map[key] = map.TryGetValue(key, out var current) ? current + weight : weight;
    }

    private static List<InterestItem> Top(Dictionary<string, int> map, int take)
        => map.OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .Take(take)
            .Select(kv => new InterestItem { Name = kv.Key, Weight = kv.Value })
            .ToList();
}
