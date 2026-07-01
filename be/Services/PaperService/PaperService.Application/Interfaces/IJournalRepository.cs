using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces;

public interface IJournalRepository
{
    Task<Journal?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Journal journal, CancellationToken cancellationToken = default);
}
