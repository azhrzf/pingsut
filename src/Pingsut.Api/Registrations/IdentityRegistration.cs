using Microsoft.EntityFrameworkCore;
using Pingsut.App.Domain;
using Pingsut.Database;

namespace Pingsut.Api.Registrations;

public static class IdentityRegistration
{
    public static void AddIdentityServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(postgresConnectionString));
        services.AddIdentityApiEndpoints<BasePlayer>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }
}