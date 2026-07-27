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
            ?? "Server=localhost,1433;Database=SyncDb;User Id=sa;Password=Prn232_Sql_Strong!2026;TrustServerCertificate=True;Encrypt=False;";

        services.Configure<OpenAlexSettings>(configuration.GetSection(OpenAlexSettings.SectionName));
        services.Configure<PaperServiceSettings>(configuration.GetSection(PaperServiceSettings.SectionName));

        services.AddDbContext<SyncDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<IOpenAlexClient, OpenAlexClient>();
        services.AddHttpClient<IPaperImportClient, PaperImportClient>();
        services.AddRabbitMqEventBus(configuration);
        services.AddTransactionalOutbox<SyncDbContext>();
        services.AddApplication();

        return services;
    }

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        await db.Database.EnsureCreatedAsync();
        await PRN232ASM.BuildingBlocks.EventBus.Outbox.OutboxSchema.EnsureCreatedAsync(db);

        // Keep DataSource.IsEnabled in sync with OpenAlex__Enabled (compose=true, local appsettings=false).
        var openAlexEnabled = string.Equals(
            configuration["OpenAlex:Enabled"],
            "true",
            StringComparison.OrdinalIgnoreCase);

        var maxImportFromConfig = configuration.GetValue<int?>("OpenAlex:MaxImportCount");
        var maxImportCount = maxImportFromConfig is > 0 ? maxImportFromConfig.Value : 303;

        var openAlex = await db.DataSources.FirstOrDefaultAsync(x => x.Name == "OpenAlex");
        if (openAlex is null)
        {
            db.DataSources.Add(new DataSource
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Name = "OpenAlex",
                BaseUrl = "https://api.openalex.org",
                ApiKey = null,
                IsEnabled = openAlexEnabled,
                MaxImportCount = maxImportCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            return;
        }

        var dirty = false;
        if (openAlex.IsEnabled != openAlexEnabled)
        {
            openAlex.IsEnabled = openAlexEnabled;
            dirty = true;
        }

        if (maxImportFromConfig is > 0 && openAlex.MaxImportCount != maxImportCount)
        {
            openAlex.MaxImportCount = maxImportCount;
            dirty = true;
        }

        if (dirty)
        {
            openAlex.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}
