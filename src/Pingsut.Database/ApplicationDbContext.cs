using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pingsut.App.Domain;

namespace Pingsut.Database;

public class ApplicationDbContext : IdentityDbContext<BasePlayer>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
    }
}