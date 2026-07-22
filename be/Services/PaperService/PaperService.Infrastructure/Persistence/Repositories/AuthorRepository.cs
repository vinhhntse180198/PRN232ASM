using Microsoft.EntityFrameworkCore;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly PaperServiceDbContext _context;

    public AuthorRepository(PaperServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _context.Authors.AddAsync(author, cancellationToken);
    }

    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Authors.AsNoTracking().OrderBy(a => a.Name).ToListAsync(cancellationToken);
    }

    public async Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Authors.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }
}
