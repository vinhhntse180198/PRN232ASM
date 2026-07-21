namespace NotificationService.Domain.Entities;

public class FollowKeyword
{
    public Guid UserId { get; set; }
    public Guid KeywordId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
