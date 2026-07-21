using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Application.EventHandlers;

public class PaperCreatedEventHandler : IIntegrationEventHandler<PaperCreatedEvent>
{
    private readonly ITrendCalculatorService _trendCalculatorService;
    private readonly ILogger<PaperCreatedEventHandler> _logger;

    public PaperCreatedEventHandler(
        ITrendCalculatorService trendCalculatorService,
        ILogger<PaperCreatedEventHandler> logger)
    {
        _trendCalculatorService = trendCalculatorService;
        _logger = logger;
    }

    public async Task HandleAsync(PaperCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling PaperCreatedEvent for paper {PaperId}", @event.PaperId);

        await _trendCalculatorService.IncrementFromPaperAsync(
            @event.TopicId,
            @event.PublicationYear,
            @event.Keywords,
            cancellationToken);
    }
}
