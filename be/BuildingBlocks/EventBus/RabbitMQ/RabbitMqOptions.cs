namespace PRN232ASM.BuildingBlocks.EventBus.RabbitMQ;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "prn232asm_event_bus";

    /// <summary>
    /// Per-service queue prefix so multiple consumers of the same event
    /// each get a copy (e.g. notification vs trend for PaperCreatedEvent).
    /// </summary>
    public string QueuePrefix { get; set; } = "default";
}
