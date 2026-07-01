using TrendService.Application.Interfaces;
using TrendService.Infrastructure.Data;

namespace TrendService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TrendDbContext _context;
    public UnitOfWork(TrendDbContext context, ITrendRepository trends) { _context = context; Trends = trends; }
    public ITrendRepository Trends { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    public void Dispose() => _context.Dispose();
}
