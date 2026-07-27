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
            ?? "Server=localhost,1433;Database=NotificationDb;User Id=sa;Password=Prn232_Sql_Strong!2026;TrustServerCertificate=True;Encrypt=False;";

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddRabbitMqEventBus(configuration);
        services.AddTransactionalOutbox<NotificationDbContext>();
        services.AddApplication();
        services.AddHostedService<BackgroundJobs.CleanupOldNotificationsJob>();

        return services;
    }
}
