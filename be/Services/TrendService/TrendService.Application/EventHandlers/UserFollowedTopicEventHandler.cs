using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.Contracts.Notifications;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;

namespace PRN232ASM.TrendService.Application.EventHandlers;

/// <summary>
/// Consumes follow events so TrendService can react to user interest (audit/telemetry path).
/// </summary>
public sealed class UserFollowedTopicEventHandler : IIntegrationEventHandler<UserFollowedTopicEvent>
{
    private readonly ILogger<UserFollowedTopicEventHandler> _logger;

    public UserFollowedTopicEventHandler(ILogger<UserFollowedTopicEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(UserFollowedTopicEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "User {UserId} followed topic {TopicId} — TrendService acknowledged follow signal.",
            @event.UserId,
            @event.TopicId);
        return Task.CompletedTask;
    }
}
