using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pingsut.App.Features.Game.Mode.NonTransitive;

namespace Pingsut.App.ServiceRegistrations;

public static class ApplicationValidatorsServiceRegistration
{
    public static void AddApplicationValidatorsServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<NonTransitiveCommandValidator>();
    }
}