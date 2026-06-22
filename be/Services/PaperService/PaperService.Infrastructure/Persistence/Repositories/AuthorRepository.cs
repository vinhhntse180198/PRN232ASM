using Microsoft.EntityFrameworkCore;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly PaperServiceDbContext _context;

    public AuthorRepository(PaperServiceDbContext context) => _context = context;

    public async Task<IReadOnlyList<Author>> SearchByNameAsync(string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        var term = query.ToLower();
        return await _context.Authors
            .AsNoTracking()
            .Where(a => a.Name.ToLower().Contains(term))
            .OrderBy(a => a.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
