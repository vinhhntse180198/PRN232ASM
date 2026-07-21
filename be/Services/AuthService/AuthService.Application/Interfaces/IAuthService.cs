using PRN232ASM.BuildingBlocks.Contracts.Auth;

namespace PRN232ASM.AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken = default);
    Task<TokenResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}
