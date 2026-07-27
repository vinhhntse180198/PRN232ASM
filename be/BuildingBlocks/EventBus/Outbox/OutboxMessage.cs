namespace PRN232ASM.BuildingBlocks.EventBus.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? LastError { get; set; }
    public int RetryCount { get; set; }
}
