using TrendService.Application.Settings;
using TrendService.Infrastructure;
using TrendService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN232ASM.Shared;
using System.Text;

var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env"));
if (File.Exists(envPath)) DotNetEnv.Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

if (SupabaseConnectionHelper.UseSupabaseDatabase())
{
    var cs = SupabaseConnectionHelper.Build(builder.Configuration, TrendDbContext.Schema);
    if (!string.IsNullOrEmpty(cs))
        builder.Configuration["ConnectionStrings:DefaultConnection"] = cs;
}
else if (builder.Environment.IsDevelopment())
    builder.Configuration["ConnectionStrings:DefaultConnection"] = "Data Source=trend.db";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(o => o.AddPolicy("Frontend", p => p.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrendDbContext>();
    if (!SupabaseConnectionHelper.UseSupabaseDatabase())
    {
        await db.Database.EnsureCreatedAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SELECT KeywordName FROM publication_trends LIMIT 1");
        }
        catch
        {
            await db.Database.ExecuteSqlRawAsync(
                "ALTER TABLE publication_trends ADD COLUMN KeywordName TEXT NULL");
        }
    }
}
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("Frontend"); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.Run();
