using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;
using PRN232ASM.TrendService.Application.EventHandlers;
using PRN232ASM.TrendService.Application.Interfaces;
using PRN232ASM.TrendService.Application.Services;
using PRN232ASM.TrendService.Infrastructure.Persistence;

namespace PRN232ASM.TrendService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTrendInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=trend.db";

        services.AddDbContext<TrendServiceDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITrendService, TrendQueryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ITrendCalculatorService, TrendCalculatorService>();

        services.Configure<PaperServiceOptions>(configuration.GetSection(PaperServiceOptions.SectionName));
        services.AddHttpClient("PaperService");

        services.AddRabbitMqEventBus(configuration);
        services.AddEventHandler<PaperCreatedEvent, PaperCreatedEventHandler>();

        return services;
    }
}
