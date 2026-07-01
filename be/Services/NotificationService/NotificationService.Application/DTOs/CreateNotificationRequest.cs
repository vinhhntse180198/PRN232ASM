namespace NotificationService.Application.DTOs;

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public Guid? PaperId { get; set; }
    public string Type { get; set; } = "SYSTEM";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
