using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;
using PRN232ASM.PaperService.Application.Interfaces;
using PRN232ASM.PaperService.Application.Services;
using PRN232ASM.PaperService.Infrastructure.Eventing;
using PRN232ASM.PaperService.Infrastructure.Grpc;
using PRN232ASM.PaperService.Infrastructure.Persistence;

namespace PRN232ASM.PaperService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaperInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=PaperDb;User Id=sa;Password=Prn232_Sql_Strong!2026;TrustServerCertificate=True;Encrypt=False;";

        services.AddDbContext<PaperServiceDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaperService, Application.Services.PaperService>();
        services.AddScoped<IPaperEventPublisher, PaperEventPublisher>();
        services.AddSingleton<IRecommendationClient, RecommendationGrpcClient>();
        services.AddSingleton<IPricingClient, PricingGrpcClient>();
        services.AddSingleton<IInferenceClient, InferenceGrpcClient>();
        services.AddSingleton<IInventoryClient, InventoryGrpcClient>();
        services.AddSingleton<IUserProfileClient, UserProfileGrpcClient>();

        services.AddRabbitMqEventBus(configuration);
        services.AddTransactionalOutbox<PaperServiceDbContext>();

        return services;
    }
}
