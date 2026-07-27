using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using PRN232ASM.PaperService.Application.Services;

namespace PRN232ASM.PaperService.Infrastructure.Eventing;

public class PaperEventPublisher : IPaperEventPublisher
{
    private readonly IOutboxWriter _outbox;

    public PaperEventPublisher(IOutboxWriter outbox)
    {
        _outbox = outbox;
    }

    public Task PublishPaperCreatedAsync(PaperCreatedEvent @event, CancellationToken cancellationToken = default)
        => _outbox.EnqueueAsync(@event, cancellationToken);
}
