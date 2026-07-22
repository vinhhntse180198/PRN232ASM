using NotificationService.Application.Interfaces;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;

namespace NotificationService.Application.EventHandlers;

public class PaperCreatedEventHandler : IIntegrationEventHandler<PaperCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IFollowRepository _followRepository;

    public PaperCreatedEventHandler(INotificationService notificationService, IFollowRepository followRepository)
    {
        _notificationService = notificationService;
        _followRepository = followRepository;
    }

    public async Task HandleAsync(PaperCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var followerIds = new HashSet<Guid>();

        foreach (var topicId in @event.TopicIds)
        {
            var topicFollowers = await _followRepository.GetFollowersByTopicIdAsync(topicId, cancellationToken);
            foreach (var id in topicFollowers) followerIds.Add(id);
        }

        foreach (var keywordId in @event.KeywordIds)
        {
            var keywordFollowers = await _followRepository.GetFollowersByKeywordIdAsync(keywordId, cancellationToken);
            foreach (var id in keywordFollowers) followerIds.Add(id);
        }

        if (@event.JournalId.HasValue)
        {
            var journalFollowers = await _followRepository.GetFollowersByJournalIdAsync(@event.JournalId.Value, cancellationToken);
            foreach (var id in journalFollowers) followerIds.Add(id);
        }

        foreach (var userId in followerIds)
        {
            await _notificationService.CreateAsync(
                userId,
                "New paper detected",
                $"A new paper \"{@event.Title}\" matches your followed interests.",
                "NewPaperDetected",
                cancellationToken);
        }
    }
}
