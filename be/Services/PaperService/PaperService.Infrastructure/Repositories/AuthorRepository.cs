using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaperService.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly PaperDbContext _context;
    public AuthorRepository(PaperDbContext context) => _context = context;

    public async Task<Author?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Authors.FirstOrDefaultAsync(
            a => a.Name.ToLower() == name.ToLower(),
            cancellationToken);

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default)
        => await _context.Authors.AddAsync(author, cancellationToken);
}
