using System.Net;
using System.Text.Json;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Common.Models;

namespace PRN232ASM.AuthService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, response) = exception switch
        {
            NotFoundException notFound => (HttpStatusCode.NotFound, ApiResponse<object>.Fail(notFound.Message)),
            ValidationException validation => (HttpStatusCode.BadRequest, ApiResponse<object>.Fail(validation.Message)),
            UnauthorizedAccessException unauthorized => (HttpStatusCode.Unauthorized, ApiResponse<object>.Fail(unauthorized.Message)),
            _ => (HttpStatusCode.InternalServerError, ApiResponse<object>.Fail("An unexpected error occurred."))
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
