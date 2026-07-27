using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.EventBus.Outbox;

/// <summary>
/// Enqueues an integration event into the local Outbox table (same DB transaction as business data).
/// </summary>
public interface IOutboxWriter
{
    Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
