using Microsoft.Extensions.DependencyInjection;

namespace Pingsut.App.ServiceRegistrations;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly));
    }
}