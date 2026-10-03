using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pingsut.Database;

namespace Pingsut.Api.Registrations;

public static class IdentityRegistration
{
  public static void AddIdentityServices( this IServiceCollection services, WebApplicationBuilder builder )
  {
    string? postgresConnectionString = builder.Configuration.GetConnectionString( "PostgresConnection" );

    services.AddDbContext<ApplicationDbContext>( options => options.UseNpgsql( postgresConnectionString ) );
    services.AddIdentityApiEndpoints<IdentityUser>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
  }
}