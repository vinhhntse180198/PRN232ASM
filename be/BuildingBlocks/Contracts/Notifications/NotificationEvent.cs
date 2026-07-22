using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.Contracts.Notifications;

public class NotificationEvent : IntegrationEvent
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
