using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.EventBus.Outbox;

public sealed class EfOutboxWriter<TContext> : IOutboxWriter
    where TContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly TContext _db;

    public EfOutboxWriter(TContext db)
    {
        _db = db;
    }

    public Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        _db.Set<OutboxMessage>().Add(new OutboxMessage
        {
            Id = @event.Id == Guid.Empty ? Guid.NewGuid() : @event.Id,
            TypeName = typeof(TEvent).AssemblyQualifiedName
                ?? typeof(TEvent).FullName
                ?? typeof(TEvent).Name,
            Payload = JsonSerializer.Serialize(@event, typeof(TEvent), JsonOptions),
            CreatedAt = DateTime.UtcNow
        });

        return Task.CompletedTask;
    }
}
