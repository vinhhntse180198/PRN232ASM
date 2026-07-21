using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using PRN232ASM.BuildingBlocks.Common.Extensions;
using PRN232ASM.TrendService.Api.Middleware;
using PRN232ASM.TrendService.Application.BackgroundJobs;
using PRN232ASM.TrendService.Application.Mappings;
using PRN232ASM.TrendService.Infrastructure;
using PRN232ASM.TrendService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5003");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(typeof(MappingProfile));
builder.Services.AddTrendInfrastructure(builder.Configuration);

builder.Services.AddHangfire(config =>
    config.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMemoryStorage());

builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrendServiceDbContext>();
    await db.Database.EnsureCreatedAsync();
}

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<TrendAggregationJob>(
    "trend-aggregation",
    job => job.ExecuteAsync(),
    "0 */6 * * *");

BackgroundJob.Enqueue<TrendAggregationJob>(job => job.ExecuteAsync());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new AllowAllDashboardAuthorizationFilter()]
});

app.MapControllers();

app.Run();

public sealed class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
