using AuthService.Application.Interfaces;
using AuthService.Application.Settings;
using AuthService.Domain.Entities;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Data;
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
    var cs = SupabaseConnectionHelper.Build(builder.Configuration, AuthDbContext.Schema);
    if (!string.IsNullOrEmpty(cs))
        builder.Configuration["ConnectionStrings:DefaultConnection"] = cs;
}
else if (builder.Environment.IsDevelopment())
    builder.Configuration["ConnectionStrings:DefaultConnection"] = "Data Source=auth.db";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    if (SupabaseConnectionHelper.UseSupabaseDatabase())
    {
        // Bảng tạo bằng docs/architecture/microservices-database-split.sql
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Full system access", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "Researcher", Description = "Analyze trends", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "Student", Description = "Search papers", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "Lecturer", Description = "View statistics", CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }

        if (!await db.Users.AnyAsync())
        {
            var studentRole = await db.Roles.FirstAsync(r => r.Name == "Student");
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                RoleId = studentRole.Id,
                FullName = "Demo User",
                Email = "user@gmail.com",
                PasswordHash = hasher.Hash("123456"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
