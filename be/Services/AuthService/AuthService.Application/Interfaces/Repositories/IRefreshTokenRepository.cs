namespace PRN232ASM.AuthService.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<Domain.Entities.RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.RefreshToken refreshToken, CancellationToken cancellationToken = default);
    void Update(Domain.Entities.RefreshToken refreshToken);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
