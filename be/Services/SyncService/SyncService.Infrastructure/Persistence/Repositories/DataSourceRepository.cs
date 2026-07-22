using Microsoft.EntityFrameworkCore;
using SyncService.Application.Interfaces.Repositories;
using SyncService.Domain.Entities;
using SyncService.Infrastructure.Persistence;

namespace SyncService.Infrastructure.Persistence.Repositories;

public class DataSourceRepository : IDataSourceRepository
{
    private readonly SyncDbContext _db;

    public DataSourceRepository(SyncDbContext db) => _db = db;

    public async Task<IReadOnlyList<DataSource>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.DataSources.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<DataSource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.DataSources.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<DataSource?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => _db.DataSources.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public Task UpdateAsync(DataSource dataSource, CancellationToken cancellationToken = default)
    {
        _db.DataSources.Update(dataSource);
        return Task.CompletedTask;
    }
}
