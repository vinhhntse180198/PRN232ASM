using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface IFollowRepository
{
    Task<FollowTopic?> GetTopicFollowAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default);
    Task AddTopicFollowAsync(FollowTopic follow, CancellationToken cancellationToken = default);
    Task RemoveTopicFollowAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default);
    Task<FollowKeyword?> GetKeywordFollowAsync(Guid userId, Guid keywordId, CancellationToken cancellationToken = default);
    Task AddKeywordFollowAsync(FollowKeyword follow, CancellationToken cancellationToken = default);
    Task RemoveKeywordFollowAsync(Guid userId, Guid keywordId, CancellationToken cancellationToken = default);
    Task<FollowJournal?> GetJournalFollowAsync(Guid userId, Guid journalId, CancellationToken cancellationToken = default);
    Task AddJournalFollowAsync(FollowJournal follow, CancellationToken cancellationToken = default);
    Task RemoveJournalFollowAsync(Guid userId, Guid journalId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetFollowedTopicIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetFollowersByTopicIdAsync(Guid topicId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetFollowersByKeywordIdAsync(Guid keywordId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetFollowersByJournalIdAsync(Guid journalId, CancellationToken cancellationToken = default);
    Task<FollowsSummary> GetFollowsSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

public record FollowsSummary(
    IReadOnlyList<Guid> TopicIds,
    IReadOnlyList<Guid> KeywordIds,
    IReadOnlyList<Guid> JournalIds);
