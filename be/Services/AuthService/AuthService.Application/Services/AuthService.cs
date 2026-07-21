using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.AuthService.Application.Interfaces.Repositories;
using PRN232ASM.AuthService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Contracts.Auth;

namespace PRN232ASM.AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponse> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken = default)
    {
        ValidateRegistration(fullName, email, password);

        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
        {
            throw new ValidationException("Email is already registered.");
        }

        var studentRole = await _roleRepository.GetByNameAsync(Role.Student, cancellationToken)
            ?? throw new NotFoundException("Role", Role.Student);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(password),
            RoleId = studentRole.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await CreateTokenResponseAsync(user, studentRole.Name, cancellationToken);
    }

    public async Task<TokenResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("Email and password are required.");
        }

        var user = await _userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email or password.");
        }

        var role = user.Role ?? await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken)
            ?? throw new NotFoundException("Role", user.RoleId);

        return await CreateTokenResponseAsync(user, role.Name, cancellationToken);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh token is required.");
        }

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken is null || !storedToken.IsActive)
        {
            throw new ValidationException("Invalid or expired refresh token.");
        }

        var user = storedToken.User ?? await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken)
            ?? throw new NotFoundException("User", storedToken.UserId);

        storedToken.IsRevoked = true;
        _refreshTokenRepository.Update(storedToken);

        var role = user.Role ?? await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken)
            ?? throw new NotFoundException("Role", user.RoleId);

        return await CreateTokenResponseAsync(user, role.Name, cancellationToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh token is required.");
        }

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken is null)
        {
            throw new NotFoundException("Refresh token not found.");
        }

        storedToken.IsRevoked = true;
        _refreshTokenRepository.Update(storedToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateRegistration(string fullName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ValidationException("Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new ValidationException("Password must be at least 8 characters.");
        }
    }

    private async Task<TokenResponse> CreateTokenResponseAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roleName);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = _jwtTokenService.GetRefreshTokenExpiration(),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        user.Role ??= new Role { Id = user.RoleId, Name = roleName };
        return new TokenResponse(
            accessToken,
            refreshTokenValue,
            _jwtTokenService.GetAccessTokenExpiration(),
            Mappings.UserMapper.ToDto(user));
    }
}
