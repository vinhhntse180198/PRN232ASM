using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaperService.Infrastructure.Repositories;

public class JournalRepository : IJournalRepository
{
    private readonly PaperDbContext _context;
    public JournalRepository(PaperDbContext context) => _context = context;

    public async Task<Journal?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Journals.FirstOrDefaultAsync(
            j => j.Name.ToLower() == name.ToLower(),
            cancellationToken);

    public async Task AddAsync(Journal journal, CancellationToken cancellationToken = default)
        => await _context.Journals.AddAsync(journal, cancellationToken);
}
