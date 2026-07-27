namespace PRN232ASM.BuildingBlocks.EventBus.Abstractions;

public sealed class EventBusPublishException : Exception
{
    public EventBusPublishException(string eventName, string reason, Exception? inner = null)
        : base($"Failed to publish integration event '{eventName}': {reason}", inner)
    {
        EventName = eventName;
    }

    public string EventName { get; }
}
