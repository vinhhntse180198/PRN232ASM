using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Interfaces.Repositories;

public interface IKeywordRepository
{
    Task<IReadOnlyList<Keyword>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Keyword?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Keyword keyword, CancellationToken cancellationToken = default);
}
