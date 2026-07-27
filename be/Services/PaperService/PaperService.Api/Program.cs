using Microsoft.AspNetCore.Server.Kestrel.Core;
using PRN232ASM.BuildingBlocks.Common.Extensions;
using PRN232ASM.PaperService.Api.Grpc;
using PRN232ASM.PaperService.Api.Middleware;
using PRN232ASM.PaperService.Application.Mappings;
using PRN232ASM.PaperService.Infrastructure;
using PRN232ASM.PaperService.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // REST (HTTP/1.1) + gRPC (HTTP/2) on the same port for Docker internal traffic.
    options.ConfigureEndpointDefaults(endpoint =>
    {
        endpoint.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddControllers();
builder.Services.AddGrpc();
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
app.MapGrpcService<PaperCatalogGrpcService>();

app.Run();
