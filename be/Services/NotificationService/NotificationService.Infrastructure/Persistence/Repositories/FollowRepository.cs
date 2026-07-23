using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly NotificationDbContext _db;

    public FollowRepository(NotificationDbContext db) => _db = db;

    public Task<FollowTopic?> GetTopicFollowAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default)
        => _db.FollowTopics.FirstOrDefaultAsync(x => x.UserId == userId && x.TopicId == topicId, cancellationToken);

    public async Task AddTopicFollowAsync(FollowTopic follow, CancellationToken cancellationToken = default)
        => await _db.FollowTopics.AddAsync(follow, cancellationToken);

    public async Task RemoveTopicFollowAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default)
    {
        var entity = await GetTopicFollowAsync(userId, topicId, cancellationToken);
        if (entity is not null)
            _db.FollowTopics.Remove(entity);
    }

    public Task<FollowKeyword?> GetKeywordFollowAsync(Guid userId, Guid keywordId, CancellationToken cancellationToken = default)
        => _db.FollowKeywords.FirstOrDefaultAsync(x => x.UserId == userId && x.KeywordId == keywordId, cancellationToken);

    public async Task AddKeywordFollowAsync(FollowKeyword follow, CancellationToken cancellationToken = default)
        => await _db.FollowKeywords.AddAsync(follow, cancellationToken);

    public async Task RemoveKeywordFollowAsync(Guid userId, Guid keywordId, CancellationToken cancellationToken = default)
    {
        var entity = await GetKeywordFollowAsync(userId, keywordId, cancellationToken);
        if (entity is not null)
            _db.FollowKeywords.Remove(entity);
    }

    public Task<FollowJournal?> GetJournalFollowAsync(Guid userId, Guid journalId, CancellationToken cancellationToken = default)
        => _db.FollowJournals.FirstOrDefaultAsync(x => x.UserId == userId && x.JournalId == journalId, cancellationToken);

    public async Task AddJournalFollowAsync(FollowJournal follow, CancellationToken cancellationToken = default)
        => await _db.FollowJournals.AddAsync(follow, cancellationToken);

    public async Task RemoveJournalFollowAsync(Guid userId, Guid journalId, CancellationToken cancellationToken = default)
    {
        var entity = await GetJournalFollowAsync(userId, journalId, cancellationToken);
        if (entity is not null)
            _db.FollowJournals.Remove(entity);
    }

    public async Task<IReadOnlyList<Guid>> GetFollowedTopicIdsAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _db.FollowTopics.Where(x => x.UserId == userId).Select(x => x.TopicId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetFollowersByTopicIdAsync(Guid topicId, CancellationToken cancellationToken = default)
        => await _db.FollowTopics.Where(x => x.TopicId == topicId).Select(x => x.UserId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetFollowersByKeywordIdAsync(Guid keywordId, CancellationToken cancellationToken = default)
        => await _db.FollowKeywords.Where(x => x.KeywordId == keywordId).Select(x => x.UserId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetFollowersByJournalIdAsync(Guid journalId, CancellationToken cancellationToken = default)
        => await _db.FollowJournals.Where(x => x.JournalId == journalId).Select(x => x.UserId).ToListAsync(cancellationToken);

    public async Task<FollowsSummary> GetFollowsSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var topics = await GetFollowedTopicIdsAsync(userId, cancellationToken);
        var keywords = await _db.FollowKeywords.Where(x => x.UserId == userId).Select(x => x.KeywordId).ToListAsync(cancellationToken);
        var journals = await _db.FollowJournals.Where(x => x.UserId == userId).Select(x => x.JournalId).ToListAsync(cancellationToken);
        return new FollowsSummary(topics, keywords, journals);
    }
}
