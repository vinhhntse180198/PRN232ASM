using NotificationService.Application.Interfaces;
using PRN232ASM.BuildingBlocks.Contracts.Trends;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;

namespace NotificationService.Application.EventHandlers;

public class TrendUpdatedEventHandler : IIntegrationEventHandler<TrendUpdatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IFollowRepository _followRepository;

    public TrendUpdatedEventHandler(INotificationService notificationService, IFollowRepository followRepository)
    {
        _notificationService = notificationService;
        _followRepository = followRepository;
    }

    public async Task HandleAsync(TrendUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var followerIds = new HashSet<Guid>();

        if (@event.TopicId != Guid.Empty)
        {
            foreach (var id in await _followRepository.GetFollowersByTopicIdAsync(@event.TopicId, cancellationToken))
                followerIds.Add(id);
        }

        if (@event.KeywordId != Guid.Empty)
        {
            foreach (var id in await _followRepository.GetFollowersByKeywordIdAsync(@event.KeywordId, cancellationToken))
                followerIds.Add(id);
        }

        var label = string.IsNullOrWhiteSpace(@event.Keyword) ? @event.TopicName : @event.Keyword;

        foreach (var userId in followerIds)
        {
            await _notificationService.CreateAsync(
                userId,
                "Trend update",
                $"\"{label}\" grew {Math.Round(@event.GrowthPercent, 1)}% in {@event.Period} ({@event.PaperCount} papers).",
                "TrendUpdated",
                cancellationToken);
        }
    }
}
