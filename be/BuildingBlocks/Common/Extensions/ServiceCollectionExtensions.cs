using Microsoft.Extensions.DependencyInjection;

namespace PRN232ASM.BuildingBlocks.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Type assemblyMarker)
    {
        services.AddAutoMapper(assemblyMarker.Assembly);
        return services;
    }
}
