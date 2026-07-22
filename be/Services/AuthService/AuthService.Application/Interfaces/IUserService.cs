using PRN232ASM.BuildingBlocks.Contracts.Auth;

namespace PRN232ASM.AuthService.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
}
