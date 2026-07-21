using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyncService.Application;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;
using SyncService.Domain.Entities;
using SyncService.Infrastructure.ExternalApis;
using SyncService.Infrastructure.Persistence;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;

namespace SyncService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=sync.db";

        services.Configure<OpenAlexSettings>(configuration.GetSection(OpenAlexSettings.SectionName));
        services.Configure<PaperServiceSettings>(configuration.GetSection(PaperServiceSettings.SectionName));

        services.AddDbContext<SyncDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<IOpenAlexClient, OpenAlexClient>();
        services.AddHttpClient<IPaperImportClient, PaperImportClient>();
        services.AddRabbitMqEventBus(configuration);
        services.AddApplication();

        return services;
    }

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncDbContext>();
        await db.Database.EnsureCreatedAsync();

        if (!await db.DataSources.AnyAsync(x => x.Name == "OpenAlex"))
        {
            db.DataSources.Add(new DataSource
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Name = "OpenAlex",
                BaseUrl = "https://api.openalex.org",
                ApiKey = null,
                IsEnabled = false,
                MaxImportCount = 303,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }
}
