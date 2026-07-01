using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces;

public interface IAuthorRepository
{
    Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Author author, CancellationToken cancellationToken = default);
}
