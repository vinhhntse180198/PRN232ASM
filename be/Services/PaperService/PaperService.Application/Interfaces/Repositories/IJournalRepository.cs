using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces.Repositories;

public interface IJournalRepository
{
    Task<IReadOnlyList<Journal>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Journal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
