using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.Contracts.Abstractions;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;

namespace PRN232ASM.BuildingBlocks.EventBus.Outbox;

/// <summary>
/// Polls OutboxMessages and publishes them to RabbitMQ. Retries on failure.
/// </summary>
public sealed class OutboxDispatcherHostedService<TContext> : BackgroundService
    where TContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxDispatcherHostedService<TContext>> _logger;

    public OutboxDispatcherHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxDispatcherHostedService<TContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox dispatcher started for {Context}.", typeof(TContext).Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DispatchBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Outbox dispatcher loop failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task DispatchBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var pending = await db.Set<OutboxMessage>()
            .Where(x => x.ProcessedAt == null && x.RetryCount < 15)
            .OrderBy(x => x.CreatedAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        if (pending.Count == 0)
            return;

        foreach (var message in pending)
        {
            try
            {
                var type = Type.GetType(message.TypeName, throwOnError: false);
                if (type is null || !typeof(IntegrationEvent).IsAssignableFrom(type))
                {
                    message.RetryCount = 15;
                    message.LastError = $"Unknown event type: {message.TypeName}";
                    _logger.LogError("Outbox message {Id} has unknown type {Type}", message.Id, message.TypeName);
                    continue;
                }

                var evt = JsonSerializer.Deserialize(message.Payload, type, JsonOptions) as IntegrationEvent;
                if (evt is null)
                {
                    message.RetryCount = 15;
                    message.LastError = "Failed to deserialize payload.";
                    continue;
                }

                await eventBus.PublishAsync(evt, cancellationToken);
                message.ProcessedAt = DateTime.UtcNow;
                message.LastError = null;
            }
            catch (Exception ex)
            {
                message.RetryCount += 1;
                message.LastError = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message;
                _logger.LogWarning(ex, "Outbox publish failed for {Id} (retry {Retry})", message.Id, message.RetryCount);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
