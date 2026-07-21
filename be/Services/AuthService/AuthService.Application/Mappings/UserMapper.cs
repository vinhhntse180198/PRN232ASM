using PRN232ASM.AuthService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Contracts.Auth;

namespace PRN232ASM.AuthService.Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(User user)
        => new(
            user.Id,
            user.Email,
            user.FullName,
            user.Role?.Name ?? string.Empty);
}
