using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.Contracts.Abstractions;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using PRN232ASM.BuildingBlocks.EventBus.RabbitMQ;

namespace PRN232ASM.BuildingBlocks.EventBus.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<RabbitMqEventBus>();
        services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<RabbitMqEventBus>());
        services.AddHostedService<EventBusInitializerHostedService>();
        return services;
    }

    /// <summary>
    /// Registers transactional outbox writer + background dispatcher for the given DbContext.
    /// Call modelBuilder.ConfigureOutboxMessages() in OnModelCreating.
    /// </summary>
    public static IServiceCollection AddTransactionalOutbox<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<IOutboxWriter, EfOutboxWriter<TContext>>();
        services.AddHostedService<OutboxDispatcherHostedService<TContext>>();
        return services;
    }

    public static IServiceCollection AddEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IIntegrationEventHandler<TEvent>>(sp => sp.GetRequiredService<THandler>());
        EventBusRegistration.Subscriptions.Add(bus => bus.Subscribe<TEvent, THandler>());
        return services;
    }
}

internal static class EventBusRegistration
{
    public static readonly List<Action<IEventBus>> Subscriptions = [];
}

internal sealed class EventBusInitializerHostedService : IHostedService
{
    private readonly RabbitMqEventBus _eventBus;
    private readonly ILogger<EventBusInitializerHostedService> _logger;

    public EventBusInitializerHostedService(
        RabbitMqEventBus eventBus,
        ILogger<EventBusInitializerHostedService> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var subscribe in EventBusRegistration.Subscriptions)
            subscribe(_eventBus);

        try
        {
            await _eventBus.InitializeAsync(cancellationToken);
            _logger.LogInformation(
                "Registered {Count} RabbitMQ event subscriptions.",
                EventBusRegistration.Subscriptions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "RabbitMQ is unavailable. Services will start without messaging. Start RabbitMQ for event-driven features.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
