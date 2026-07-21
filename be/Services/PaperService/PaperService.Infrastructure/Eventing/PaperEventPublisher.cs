using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;
using PRN232ASM.PaperService.Application.Services;

namespace PRN232ASM.PaperService.Infrastructure.Eventing;

public class PaperEventPublisher : IPaperEventPublisher
{
    private readonly IEventBus _eventBus;

    public PaperEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task PublishPaperCreatedAsync(PaperCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(@event, cancellationToken);
    }
}
