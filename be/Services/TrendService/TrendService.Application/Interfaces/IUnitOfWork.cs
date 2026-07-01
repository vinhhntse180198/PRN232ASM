namespace TrendService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITrendRepository Trends { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
