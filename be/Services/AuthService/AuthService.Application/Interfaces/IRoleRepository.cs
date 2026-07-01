using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
