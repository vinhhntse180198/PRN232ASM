using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.AuthService.Application.Interfaces.Repositories;
using PRN232ASM.AuthService.Application.Mappings;
using PRN232ASM.AuthService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Contracts.Auth;

namespace PRN232ASM.AuthService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(UserMapper.ToDto).ToList();
    }

    public async Task<UserDto> UpdateUserRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ValidationException("Role is required.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        var role = await _roleRepository.GetByNameAsync(roleName.Trim(), cancellationToken)
            ?? throw new NotFoundException("Role", roleName);

        if (!IsValidRole(role.Name))
        {
            throw new ValidationException($"Invalid role '{roleName}'. Allowed roles: Admin, Researcher, Student.");
        }

        user.RoleId = role.Id;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        user.Role = role;
        return UserMapper.ToDto(user);
    }

    private static bool IsValidRole(string roleName)
        => roleName is Role.Admin or Role.Researcher or Role.Student;
}
