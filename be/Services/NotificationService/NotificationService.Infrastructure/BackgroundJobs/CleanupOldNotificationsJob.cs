using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.BackgroundJobs;

public class CleanupOldNotificationsJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupOldNotificationsJob> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromDays(7);
    private const int RetentionDays = 30;

    public CleanupOldNotificationsJob(IServiceProvider serviceProvider, ILogger<CleanupOldNotificationsJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);
                var old = await db.Notifications
                    .Where(n => n.IsRead && n.CreatedAt < cutoff)
                    .ToListAsync(stoppingToken);

                if (old.Count > 0)
                {
                    db.Notifications.RemoveRange(old);
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("CleanupOldNotificationsJob removed {Count} notifications.", old.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupOldNotificationsJob failed.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
