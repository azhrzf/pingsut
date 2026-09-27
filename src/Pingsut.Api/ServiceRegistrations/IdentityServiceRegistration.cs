using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pingsut.Database;

namespace Pingsut.Api.ServiceRegistrations;

public static class IdentityServiceRegistration
{
    public static void AddIdentityServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(postgresConnectionString));
        services.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }
}