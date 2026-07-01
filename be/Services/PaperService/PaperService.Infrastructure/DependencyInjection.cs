using PaperService.Application.Interfaces;
using PaperService.Application.Services;
using PaperService.Application.Settings;
using PaperService.Infrastructure.Data;
using PaperService.Infrastructure.Persistence;
using PaperService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaperService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=paper.db";
        services.AddDbContext<PaperDbContext>(options =>
        {
            if (IsSqlite(connectionString)) options.UseSqlite(connectionString);
            else options.UseNpgsql(connectionString);
        });

        services.AddScoped<IPaperRepository, PaperRepository>();
        services.AddScoped<IJournalRepository, JournalRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IKeywordRepository, KeywordRepository>();
        services.AddScoped<IBookmarkRepository, BookmarkRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaperService, PaperAppService>();
        services.AddScoped<IBookmarkService, BookmarkAppService>();
        services.AddScoped<IAnalyticsService, AnalyticsAppService>();

        return services;
    }

    private static bool IsSqlite(string cs)
        => cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
           || cs.Contains("Filename=", StringComparison.OrdinalIgnoreCase);
}
