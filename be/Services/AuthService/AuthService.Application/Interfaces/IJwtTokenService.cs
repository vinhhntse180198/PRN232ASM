using PRN232ASM.AuthService.Domain.Entities;

namespace PRN232ASM.AuthService.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, string roleName);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiration();
    DateTime GetRefreshTokenExpiration();
}
