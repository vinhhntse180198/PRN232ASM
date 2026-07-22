using Microsoft.EntityFrameworkCore;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

public class KeywordRepository : IKeywordRepository
{
    private readonly PaperServiceDbContext _context;

    public KeywordRepository(PaperServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Keyword keyword, CancellationToken cancellationToken = default)
    {
        await _context.Keywords.AddAsync(keyword, cancellationToken);
    }

    public async Task<IReadOnlyList<Keyword>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Keywords.AsNoTracking().OrderBy(k => k.Name).ToListAsync(cancellationToken);
    }

    public async Task<Keyword?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Keywords.FirstOrDefaultAsync(k => k.Name == name, cancellationToken);
    }
}
