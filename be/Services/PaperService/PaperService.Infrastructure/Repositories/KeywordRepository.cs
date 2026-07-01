using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaperService.Infrastructure.Repositories;

public class KeywordRepository : IKeywordRepository
{
    private readonly PaperDbContext _context;
    public KeywordRepository(PaperDbContext context) => _context = context;

    public async Task<Keyword?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Keywords.FirstOrDefaultAsync(
            k => k.Name.ToLower() == name.ToLower(),
            cancellationToken);

    public async Task AddAsync(Keyword keyword, CancellationToken cancellationToken = default)
        => await _context.Keywords.AddAsync(keyword, cancellationToken);
}
