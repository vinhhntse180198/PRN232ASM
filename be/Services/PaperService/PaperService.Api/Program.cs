using PRN232ASM.BuildingBlocks.Common.Extensions;
using PRN232ASM.PaperService.Api.Middleware;
using PRN232ASM.PaperService.Application.Mappings;
using PRN232ASM.PaperService.Infrastructure;
using PRN232ASM.PaperService.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5002");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(typeof(MappingProfile));
builder.Services.AddPaperInfrastructure(builder.Configuration);

var app = builder.Build();

await PaperDataSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
