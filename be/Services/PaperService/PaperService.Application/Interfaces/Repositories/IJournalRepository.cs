using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Interfaces.Repositories;

public interface IJournalRepository
{
    Task<IReadOnlyList<Journal>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Journal?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Journal journal, CancellationToken cancellationToken = default);
}
