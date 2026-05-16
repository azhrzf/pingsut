using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pingsut.App.Contracts.Persistence;
using Pingsut.Persistence.DbContexts;
using Pingsut.Persistence.Repositories;

namespace Pingsut.Persistence.ServiceRegistrations;

public static class PersistenceServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PostgreDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));

        services.AddScoped<IUserRepository, UserRepository>();
    }
}