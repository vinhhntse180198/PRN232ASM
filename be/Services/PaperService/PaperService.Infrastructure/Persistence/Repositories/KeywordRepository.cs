using Microsoft.EntityFrameworkCore;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Repositories;

public class KeywordRepository : IKeywordRepository
{
    private readonly PaperServiceDbContext _context;

    public KeywordRepository(PaperServiceDbContext context) => _context = context;

    public async Task<IReadOnlyList<Keyword>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Keywords
            .AsNoTracking()
            .Include(k => k.PaperKeywords)
            .OrderBy(k => k.Name)
            .ToListAsync(cancellationToken);
}
