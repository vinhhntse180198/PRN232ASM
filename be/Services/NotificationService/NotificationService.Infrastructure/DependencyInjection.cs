using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Persistence;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=notify.db";

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddRabbitMqEventBus(configuration);
        services.AddApplication();
        services.AddHostedService<BackgroundJobs.CleanupOldNotificationsJob>();

        return services;
    }
}
