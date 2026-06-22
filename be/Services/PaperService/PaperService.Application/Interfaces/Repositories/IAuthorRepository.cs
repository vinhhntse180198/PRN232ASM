using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces.Repositories;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> SearchByNameAsync(string query, int limit = 10, CancellationToken cancellationToken = default);
}
