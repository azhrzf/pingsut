using Pingsut.App.Contracts.Persistence;
using Pingsut.Persistence.DbContexts;

namespace Pingsut.Persistence.Repositories;

public class BaseRepository<T> : IAsyncRepository<T> where T : class
{
    protected readonly PostgreDbContext PostgreDbContext;

    public BaseRepository(PostgreDbContext postgreDbContext)
    {
        PostgreDbContext = postgreDbContext;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await PostgreDbContext.Set<T>().FindAsync(id, cancellationToken, cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await PostgreDbContext.Set<T>().AddAsync(entity, cancellationToken);

        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        PostgreDbContext.Set<T>().Update(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        PostgreDbContext.Set<T>().Remove(entity);

        return Task.CompletedTask;
    }
}