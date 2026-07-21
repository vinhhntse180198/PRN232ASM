namespace NotificationService.Domain.Entities;

public class FollowTopic
{
    public Guid UserId { get; set; }
    public Guid TopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
