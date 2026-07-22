namespace PRN232ASM.AuthService.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Domain.Entities.Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.Role role, CancellationToken cancellationToken = default);
}
