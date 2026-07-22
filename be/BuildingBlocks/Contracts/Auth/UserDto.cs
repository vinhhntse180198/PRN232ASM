namespace PRN232ASM.BuildingBlocks.Contracts.Auth;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role);
