using PRN232ASM.InventoryService.Services;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5009");
}

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<InventoryGrpcService>();
app.MapGet("/", () =>
    "InventoryService gRPC is running (service: inventory.JournalInventory).");

app.Run();
