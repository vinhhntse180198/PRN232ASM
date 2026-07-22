using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Interfaces.Repositories;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Author author, CancellationToken cancellationToken = default);
}
