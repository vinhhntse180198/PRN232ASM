namespace PRN232ASM.BuildingBlocks.Contracts.Abstractions;

public abstract class IntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}
