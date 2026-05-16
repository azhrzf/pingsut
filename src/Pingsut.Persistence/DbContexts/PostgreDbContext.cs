using Microsoft.EntityFrameworkCore;
using Pingsut.Domain.Entities;

namespace Pingsut.Persistence.DbContexts;

public class PostgreDbContext(DbContextOptions<PostgreDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgreDbContext).Assembly);
    }
}