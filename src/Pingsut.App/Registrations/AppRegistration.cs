using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Pingsut.App.Features.NonTransitive;
using Pingsut.App.Services;

namespace Pingsut.App.Registrations;

public static class AppRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<INonTransitiveService, NonTransitiveService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<NonTransitiveRoomManager>();

        services.AddValidatorsFromAssemblyContaining<NonTransitiveCommandValidator>();

        var mapperConfig = TypeAdapterConfig.GlobalSettings;
        mapperConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(mapperConfig);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}