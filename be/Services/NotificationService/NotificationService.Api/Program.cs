using NotificationService.Application.Settings;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PRN232ASM.Shared;
using System.Text;

var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env"));
if (File.Exists(envPath)) DotNetEnv.Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

if (SupabaseConnectionHelper.UseSupabaseDatabase())
{
    var cs = SupabaseConnectionHelper.Build(builder.Configuration, NotificationDbContext.Schema);
    if (!string.IsNullOrEmpty(cs))
        builder.Configuration["ConnectionStrings:DefaultConnection"] = cs;
}
else if (builder.Environment.IsDevelopment())
    builder.Configuration["ConnectionStrings:DefaultConnection"] = "Data Source=notify.db";

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
    if (!SupabaseConnectionHelper.UseSupabaseDatabase())
        await scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.EnsureCreatedAsync();
}
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("Frontend"); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.Run();
