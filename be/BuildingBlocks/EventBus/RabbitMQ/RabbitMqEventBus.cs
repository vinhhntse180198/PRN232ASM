using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PRN232ASM.BuildingBlocks.Contracts.Abstractions;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PRN232ASM.BuildingBlocks.EventBus.RabbitMQ;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly RabbitMqOptions _options;
    private readonly ConcurrentDictionary<string, Type> _eventTypes = new();
    private readonly ConcurrentDictionary<string, List<Type>> _handlers = new();
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public RabbitMqEventBus(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqEventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        _eventTypes[eventName] = typeof(TEvent);

        var handlers = _handlers.GetOrAdd(eventName, _ => []);
        if (!handlers.Contains(typeof(THandler)))
            handlers.Add(typeof(THandler));
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
        => EnsureInitializedAsync(cancellationToken);

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
        => PublishCoreAsync(@event, typeof(TEvent), cancellationToken);

    public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default)
        => PublishCoreAsync(@event, @event.GetType(), cancellationToken);

    private async Task PublishCoreAsync(IntegrationEvent @event, Type eventType, CancellationToken cancellationToken)
    {
        var eventName = eventType.Name;

        try
        {
            await EnsureInitializedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventName}: RabbitMQ is unavailable.", eventName);
            throw new EventBusPublishException(eventName, "RabbitMQ is unavailable.", ex);
        }

        if (_channel is null)
        {
            _logger.LogError("Failed to publish {EventName}: RabbitMQ channel is not ready.", eventName);
            throw new EventBusPublishException(eventName, "RabbitMQ channel is not ready.");
        }

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, eventType, JsonOptions));

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = @event.Id.ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        try
        {
            await _channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: eventName,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventName} with id {EventId}", eventName, @event.Id);
            throw new EventBusPublishException(eventName, "BasicPublish failed.", ex);
        }

        _logger.LogInformation("Published event {EventName} with id {EventId}", eventName, @event.Id);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized) return;

        await _initLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized) return;

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            foreach (var eventName in _handlers.Keys)
            {
                // One queue per service+event so competing consumers do not steal messages.
                var prefix = string.IsNullOrWhiteSpace(_options.QueuePrefix) ? "default" : _options.QueuePrefix.Trim();
                var queueName = $"{_options.ExchangeName}_{prefix}_{eventName}";
                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: _options.ExchangeName,
                    routingKey: eventName,
                    cancellationToken: cancellationToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, args) =>
                {
                    var name = args.RoutingKey;
                    try
                    {
                        if (!_eventTypes.TryGetValue(name, out var eventType))
                            return;

                        var json = Encoding.UTF8.GetString(args.Body.ToArray());
                        var @event = JsonSerializer.Deserialize(json, eventType, JsonOptions) as IntegrationEvent;
                        if (@event is null) return;

                        if (_handlers.TryGetValue(name, out var handlerTypes))
                        {
                            using var scope = _serviceProvider.CreateScope();
                            foreach (var handlerType in handlerTypes)
                            {
                                var handler = scope.ServiceProvider.GetService(handlerType);
                                if (handler is null) continue;

                                var handlerInterface = handlerType
                                    .GetInterfaces()
                                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

                                if (handlerInterface is null) continue;

                                var method = handlerInterface.GetMethod(nameof(IIntegrationEventHandler<IntegrationEvent>.HandleAsync));
                                if (method is not null)
                                    await (Task)method.Invoke(handler, [@event, CancellationToken.None])!;
                            }
                        }

                        await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling event {EventName}", name);
                        await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
                    }
                };

                await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer, cancellationToken);
            }

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _initLock.Dispose();
    }
}
