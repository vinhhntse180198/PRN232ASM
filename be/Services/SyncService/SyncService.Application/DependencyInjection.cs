using Microsoft.Extensions.DependencyInjection;
using SyncService.Application.Interfaces;
using SyncService.Application.Mappings;
using SyncService.Application.Services;

namespace SyncService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<ISyncService, SyncAppService>();
        services.AddScoped<BackgroundJobs.ScheduledSyncJob>();
        return services;
    }
}
