namespace NotificationService.Domain.Entities;

public class FollowJournal
{
    public Guid UserId { get; set; }
    public Guid JournalId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
