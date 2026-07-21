using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;

namespace SyncService.Application.BackgroundJobs;

public class ScheduledSyncJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<OpenAlexSettings> _openAlexOptions;
    private readonly ILogger<ScheduledSyncJob> _logger;

    public ScheduledSyncJob(
        IServiceScopeFactory scopeFactory,
        IOptions<OpenAlexSettings> openAlexOptions,
        ILogger<ScheduledSyncJob> logger)
    {
        _scopeFactory = scopeFactory;
        _openAlexOptions = openAlexOptions;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        if (!_openAlexOptions.Value.Enabled)
        {
            _logger.LogInformation("Scheduled OpenAlex sync skipped because OpenAlex is disabled.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

        try
        {
            await syncService.TriggerSyncAsync();
            _logger.LogInformation("Scheduled OpenAlex sync completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scheduled OpenAlex sync failed.");
        }
    }
}
