using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;
using PRN232ASM.PaperService.Application.Interfaces;
using PRN232ASM.PaperService.Application.Services;
using PRN232ASM.PaperService.Infrastructure.Eventing;
using PRN232ASM.PaperService.Infrastructure.Persistence;

namespace PRN232ASM.PaperService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaperInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=paper.db";

        services.AddDbContext<PaperServiceDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaperService, Application.Services.PaperService>();
        services.AddScoped<IPaperEventPublisher, PaperEventPublisher>();

        services.AddRabbitMqEventBus(configuration);

        return services;
    }
}
