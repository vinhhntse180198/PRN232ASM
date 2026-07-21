using Microsoft.Extensions.DependencyInjection;
using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.AuthService.Application.Services;

namespace PRN232ASM.AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
