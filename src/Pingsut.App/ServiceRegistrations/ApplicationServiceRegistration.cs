using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pingsut.App.Contracts;
using Pingsut.App.Features.NonTransitive;

namespace Pingsut.App.ServiceRegistrations;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<INonTransitiveService, NonTransitiveService>();
        services.AddSingleton<NonTransitiveRoomManager>();

        services.AddValidatorsFromAssemblyContaining<NonTransitiveCommandValidator>();
    }
}