using SyncService.Application.Interfaces;
using SyncService.Application.Services;
using SyncService.Application.Settings;
using SyncService.Infrastructure.Data;
using SyncService.Infrastructure.External;
using SyncService.Infrastructure.Persistence;
using SyncService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SyncService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<OpenAlexSettings>(configuration.GetSection(OpenAlexSettings.SectionName));
        services.Configure<PaperServiceSettings>(configuration.GetSection(PaperServiceSettings.SectionName));

        var cs = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=sync.db";
        services.AddDbContext<SyncDbContext>(o => { if (IsSqlite(cs)) o.UseSqlite(cs); else o.UseNpgsql(cs); });

        services.AddHttpClient<IOpenAlexClient, OpenAlexClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PRN232ASM-SyncService/1.0");
        });
        services.AddHttpClient<IPaperImportClient, PaperImportClient>();
        services.AddHttpClient<IDoiResolver, DoiResolver>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(45);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PRN232ASM/1.0)");
        });
        services.AddHttpClient<PdfProxyService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PRN232ASM/1.0)");
        });

        services.AddMemoryCache();
        services.AddScoped<ISyncJobRepository, SyncJobRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISyncService, SyncAppService>();
        services.AddScoped<IOpenAlexSearchService, OpenAlexSearchService>();
        return services;
    }

    private static bool IsSqlite(string cs) => cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase);
}
