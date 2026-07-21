using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _db;

    public NotificationRepository(NotificationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _db.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        => await _db.Notifications.AddAsync(notification, cancellationToken);

    public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (notification is not null)
            notification.IsRead = true;
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _db.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true), cancellationToken);
    }
}
