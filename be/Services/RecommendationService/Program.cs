using PRN232ASM.RecommendationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(builder.Configuration["Urls"] ?? "http://localhost:5006");

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<RecommendationGrpcService>();
app.MapGet("/", () =>
    "RecommendationService gRPC is running. Use a gRPC client on port 5006 (service: recommendation.PaperRecommender).");

app.Run();
