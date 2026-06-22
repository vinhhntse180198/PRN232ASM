using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces.Repositories;

public interface IKeywordRepository
{
    Task<IReadOnlyList<Keyword>> GetAllAsync(CancellationToken cancellationToken = default);
}
