using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.AuthService.Application.DTOs;
using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.BuildingBlocks.Common.Models;

namespace PRN232ASM.AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _authService.RegisterAsync(request.FullName, request.Email, request.Password, cancellationToken);
        return Ok(ApiResponse<object>.Ok(tokens, "Registration successful"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return Ok(ApiResponse<object>.Ok(tokens, "Login successful"));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(ApiResponse<object>.Ok(tokens, "Token refreshed successfully"));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request.RefreshToken, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Logout successful"));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Password changed successfully."));
    }

    private Guid GetUserId()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var headerValue) &&
            Guid.TryParse(headerValue.FirstOrDefault(), out var userId) &&
            userId != Guid.Empty)
        {
            return userId;
        }
        throw new UnauthorizedAccessException("X-User-Id header is required.");
    }
}
