using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.EventHandlers;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Mappings;
using NotificationService.Application.Services;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.Contracts.Trends;
using PRN232ASM.BuildingBlocks.EventBus.Extensions;

namespace NotificationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<INotificationService, NotificationAppService>();
        services.AddScoped<IFollowService, FollowAppService>();
        services.AddScoped<IFollowRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Follows);

        services.AddEventHandler<PaperCreatedEvent, PaperCreatedEventHandler>();
        services.AddEventHandler<NewPaperDetectedEvent, NewPaperDetectedEventHandler>();
        services.AddEventHandler<TrendUpdatedEvent, TrendUpdatedEventHandler>();

        return services;
    }
}
