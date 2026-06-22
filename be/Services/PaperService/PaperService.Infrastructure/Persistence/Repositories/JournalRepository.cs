using Microsoft.EntityFrameworkCore;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Repositories;

public class JournalRepository : IJournalRepository
{
    private readonly PaperServiceDbContext _context;

    public JournalRepository(PaperServiceDbContext context) => _context = context;

    public async Task<IReadOnlyList<Journal>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Journals
            .AsNoTracking()
            .Include(j => j.Papers)
            .OrderBy(j => j.Name)
            .ToListAsync(cancellationToken);

    public Task<Journal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Journals
            .Include(j => j.Papers)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
}
