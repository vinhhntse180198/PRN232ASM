namespace PRN232ASM.AuthService.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Domain.Entities.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
    void Update(Domain.Entities.User user);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
