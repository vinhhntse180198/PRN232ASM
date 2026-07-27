using PRN232ASM.PricingService.Services;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5007");
}

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<PricingGrpcService>();
app.MapGet("/", () =>
    "PricingService gRPC is running (service: pricing.PaperPricer).");

app.Run();
