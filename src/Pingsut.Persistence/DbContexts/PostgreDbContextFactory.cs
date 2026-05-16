using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pingsut.Persistence.DbContexts;

public class PostgreDbContextFactory : IDesignTimeDbContextFactory<PostgreDbContext>
{
    public PostgreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PostgreDbContext>();

        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "KendalaNet.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgreDbContext(optionsBuilder.Options);
    }
}