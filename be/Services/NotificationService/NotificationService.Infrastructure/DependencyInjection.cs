using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Application.Settings;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        var cs = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=notify.db";
        services.AddDbContext<NotificationDbContext>(o => { if (IsSqlite(cs)) o.UseSqlite(cs); else o.UseNpgsql(cs); });
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<INotificationService, NotificationAppService>();
        return services;
    }

    private static bool IsSqlite(string cs) => cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase);
}
