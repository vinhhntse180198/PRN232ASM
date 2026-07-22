using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"));
if (File.Exists(envPath))
    DotNetEnv.Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, "Configuration", "yarp.json"),
    optional: false,
    reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var jwtSecret = builder.Configuration["JWT:Secret"]
    ?? builder.Configuration["JWT__Secret"]
    ?? throw new InvalidOperationException("JWT secret is not configured.");

var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? builder.Configuration["JWT__Issuer"] ?? "PRN232ASM";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? builder.Configuration["JWT__Audience"] ?? "PRN232ASM-Users";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtValidationMiddleware>();
app.MapControllers();
app.MapReverseProxy();
app.Run();
