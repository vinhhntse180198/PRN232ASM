using SyncService.Domain.Entities;

namespace SyncService.Application.Interfaces.Repositories;

public interface IDataSourceRepository
{
    Task<IReadOnlyList<DataSource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DataSource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DataSource?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task UpdateAsync(DataSource dataSource, CancellationToken cancellationToken = default);
}
