using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Pingsut.Api.Handlers;
using Pingsut.Api.Utils;

namespace Pingsut.Api.Registrations;

public static class ApiRegistration
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

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Extensions.TryAdd(
                    "traceId",
                    System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            };
        });
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
        services.AddAuthentication();
        services.AddAuthorization();
    }
}