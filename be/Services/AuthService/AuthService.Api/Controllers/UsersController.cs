using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.AuthService.Application.DTOs;
using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.AuthService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.BuildingBlocks.Contracts.Auth;

namespace PRN232ASM.AuthService.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = Role.Admin)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UserDto>>.Ok(users));
    }

    [HttpPut("{id:guid}/role")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateUserRoleAsync(id, request.Role, cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(user, "User role updated successfully"));
    }
}
