using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Data;

namespace AuthService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;

    public UnitOfWork(AuthDbContext context, IUserRepository users, IRoleRepository roles)
    {
        _context = context;
        Users = users;
        Roles = roles;
    }

    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
