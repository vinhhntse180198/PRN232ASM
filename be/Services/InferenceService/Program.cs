using PRN232ASM.InferenceService.Services;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5008");
}

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<InferenceGrpcService>();
app.MapGet("/", () =>
    "InferenceService gRPC is running (service: inference.PaperInference).");

app.Run();
