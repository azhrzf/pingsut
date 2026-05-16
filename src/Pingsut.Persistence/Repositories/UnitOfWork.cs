using Pingsut.App.Contracts.Persistence;
using Pingsut.Persistence.DbContexts;

namespace Pingsut.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PostgreDbContext _context;

    public UnitOfWork(PostgreDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}