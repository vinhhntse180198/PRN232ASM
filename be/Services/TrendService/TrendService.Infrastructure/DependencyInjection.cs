using TrendService.Application.Interfaces;
using TrendService.Application.Services;
using TrendService.Application.Settings;
using TrendService.Infrastructure.Data;
using TrendService.Infrastructure.External;
using TrendService.Infrastructure.Persistence;
using TrendService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrendService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<PaperServiceSettings>(configuration.GetSection(PaperServiceSettings.SectionName));

        var cs = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=trend.db";
        services.AddDbContext<TrendDbContext>(o => { if (IsSqlite(cs)) o.UseSqlite(cs); else o.UseNpgsql(cs); });

        services.AddHttpClient<IPaperAnalyticsClient, PaperAnalyticsClient>();

        services.AddScoped<ITrendRepository, TrendRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITrendService, TrendAppService>();
        return services;
    }

    private static bool IsSqlite(string cs) => cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase);
}
