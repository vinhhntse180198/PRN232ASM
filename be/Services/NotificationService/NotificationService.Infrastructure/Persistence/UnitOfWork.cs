using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly NotificationDbContext _context;
    public UnitOfWork(NotificationDbContext context, INotificationRepository notifications) { _context = context; Notifications = notifications; }
    public INotificationRepository Notifications { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    public void Dispose() => _context.Dispose();
}
