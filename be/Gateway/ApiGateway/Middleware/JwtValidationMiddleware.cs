using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway.Middleware;

public class JwtValidationMiddleware
{
    private static readonly string[] PublicPaths =
    [
        "/health",
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/refresh-token",
        "/swagger"
    ];

    private readonly RequestDelegate _next;

    public JwtValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsPublicPath(path) || !path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Unauthorized." });
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? context.User.FindFirstValue("userId");

        if (!string.IsNullOrWhiteSpace(userId))
            context.Request.Headers["X-User-Id"] = userId;

        await _next(context);
    }

    private static bool IsPublicPath(string path)
        => PublicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
}
