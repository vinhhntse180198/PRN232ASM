namespace SyncService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ISyncJobRepository SyncJobs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
