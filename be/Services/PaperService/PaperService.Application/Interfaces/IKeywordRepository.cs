using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces;

public interface IKeywordRepository
{
    Task<Keyword?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Keyword keyword, CancellationToken cancellationToken = default);
}
