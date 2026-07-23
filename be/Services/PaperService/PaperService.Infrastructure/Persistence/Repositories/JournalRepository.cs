using Microsoft.EntityFrameworkCore;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

public class JournalRepository : IJournalRepository
{
    private readonly PaperServiceDbContext _context;

    public JournalRepository(PaperServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Journal journal, CancellationToken cancellationToken = default)
    {
        await _context.Journals.AddAsync(journal, cancellationToken);
    }

    public async Task<IReadOnlyList<Journal>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Journals.AsNoTracking().OrderBy(j => j.Name).ToListAsync(cancellationToken);
    }

    public async Task<Journal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Journals.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<Journal?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Journals.FirstOrDefaultAsync(j => j.Name == name, cancellationToken);
    }
}
