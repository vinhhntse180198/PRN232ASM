using PRN232ASM.UserProfileService.Services;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5010");
}

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UserProfileGrpcService>();
app.MapGet("/", () =>
    "UserProfileService gRPC is running (service: userprofile.ReaderProfiler).");

app.Run();
