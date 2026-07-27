using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRN232ASM.BuildingBlocks.Contracts.Notifications;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;
using PRN232ASM.BuildingBlocks.GrpcContracts.Papers;
using PRN232ASM.TrendService.Application.EventHandlers;
using PRN232ASM.TrendService.Application.Interfaces;
using PRN232ASM.TrendService.Application.Services;
using PRN232ASM.TrendService.Infrastructure.Grpc;
using PRN232ASM.TrendService.Infrastructure.Persistence;

namespace PRN232ASM.TrendService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTrendInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=TrendDb;User Id=sa;Password=Prn232_Sql_Strong!2026;TrustServerCertificate=True;Encrypt=False;";

        services.AddDbContext<TrendServiceDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITrendService, TrendQueryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ITrendCalculatorService, TrendCalculatorService>();

        services.Configure<PaperServiceOptions>(configuration.GetSection(PaperServiceOptions.SectionName));

        var paperSection = configuration.GetSection(PaperServiceOptions.SectionName);
        var grpcUrl = paperSection["GrpcUrl"];
        if (string.IsNullOrWhiteSpace(grpcUrl))
            grpcUrl = paperSection["BaseUrl"] ?? "http://localhost:5002";

        services.AddGrpcClient<PaperCatalog.PaperCatalogClient>(options =>
        {
            options.Address = new Uri(grpcUrl.TrimEnd('/') + "/");
        });
        services.AddScoped<IPaperCatalogClient, PaperCatalogGrpcClient>();

        services.AddRabbitMqEventBus(configuration);
        services.AddTransactionalOutbox<TrendServiceDbContext>();
        services.AddEventHandler<PaperCreatedEvent, PaperCreatedEventHandler>();
        services.AddEventHandler<UserFollowedTopicEvent, UserFollowedTopicEventHandler>();

        return services;
    }
}
