using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.AuthService.Infrastructure.Persistence;

namespace PRN232ASM.AuthService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;

    public UnitOfWork(AuthDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
