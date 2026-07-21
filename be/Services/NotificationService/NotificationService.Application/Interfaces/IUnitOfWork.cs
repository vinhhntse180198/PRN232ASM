namespace NotificationService.Application.Interfaces;

public interface IUnitOfWork
{
    INotificationRepository Notifications { get; }
    IFollowRepository Follows { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
