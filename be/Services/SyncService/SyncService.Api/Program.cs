using Hangfire;
using Hangfire.MemoryStorage;
using SyncService.Api.Middleware;
using SyncService.Application.BackgroundJobs;
using SyncService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

if (!string.Equals(builder.Configuration["USE_SUPABASE_DB"], "true", StringComparison.OrdinalIgnoreCase))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:DefaultConnection"] = "Data Source=sync.db"
    });
}

if (!string.Equals(builder.Configuration["OpenAlex:Enabled"], "true", StringComparison.OrdinalIgnoreCase))
{
    builder.Configuration["OpenAlex:Enabled"] = "false";
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHangfire(config => config.UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings()
    .UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

await DependencyInjection.SeedAsync(app.Services);

var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobs.AddOrUpdate<ScheduledSyncJob>(
    "daily-openalex-sync",
    job => job.ExecuteAsync(),
    Cron.Daily);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Frontend");
app.MapControllers();
app.Run();
