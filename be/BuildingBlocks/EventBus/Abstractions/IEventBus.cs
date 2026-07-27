using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;

    /// <summary>Publish using the runtime event type (used by Outbox dispatcher).</summary>
    Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default);

    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}
