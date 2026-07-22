using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Persistence.Repositories;

namespace NotificationService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly NotificationDbContext _db;

    public UnitOfWork(NotificationDbContext db)
    {
        _db = db;
        Notifications = new NotificationRepository(db);
        Follows = new FollowRepository(db);
    }

    public INotificationRepository Notifications { get; }
    public IFollowRepository Follows { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
