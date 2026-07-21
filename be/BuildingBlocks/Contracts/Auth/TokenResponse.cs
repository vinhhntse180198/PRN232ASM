namespace PRN232ASM.BuildingBlocks.Contracts.Auth;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);
