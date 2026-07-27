using PRN232ASM.BuildingBlocks.EventBus.RabbitMQ;
using Xunit;

namespace TrendService.Tests;

public class RabbitMqQueueNamingTests
{
    [Fact]
    public void QueuePrefix_is_included_in_queue_name()
    {
        var options = new RabbitMqOptions
        {
            ExchangeName = "prn232asm_event_bus",
            QueuePrefix = "trend"
        };

        var eventName = "PaperCreatedEvent";
        var prefix = string.IsNullOrWhiteSpace(options.QueuePrefix) ? "default" : options.QueuePrefix.Trim();
        var queueName = $"{options.ExchangeName}_{prefix}_{eventName}";

        Assert.Equal("prn232asm_event_bus_trend_PaperCreatedEvent", queueName);
        Assert.NotEqual("prn232asm_event_bus_PaperCreatedEvent", queueName);
    }
}
