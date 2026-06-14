using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Pingsut.Api.Utils;

namespace Pingsut.Api.ServiceRegistrations;

public static class ApiServiceRegistration
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSignalR();
        services.AddControllers(options =>
        {
            options.Conventions.Add(
                new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        }).AddNewtonsoftJson();

        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        services.AddApiVersioning(setupAction =>
            {
                setupAction.ReportApiVersions = true;
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }
}