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
        var followers = await _followRepository.GetFollowersByTopicIdAsync(@event.TopicId, cancellationToken);

        foreach (var userId in followers)
        {
            await _notificationService.CreateAsync(
                userId,
                "Trend update",
                $"Topic \"{@event.TopicName}\" grew {Math.Round(@event.GrowthPercent, 1)}% in {@event.Period} ({@event.PaperCount} papers).",
                "TrendUpdated",
                cancellationToken);
        }
    }
}
