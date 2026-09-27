using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Pingsut.Api.Handlers;

internal static class ProblemDetailsMapper
{
    static ProblemDetailsMapper()
    {
        Map<ValidationException>(exception => new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = exception.Message
        });

        Map<UnauthorizedAccessException>(exception => new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = exception.Message
        });

        Map<KeyNotFoundException>(exception => new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = exception.Message
        });
    }

    public static ProblemDetails Map(Exception exception)
    {
        foreach (var mapper in Mappers)
        {
            if (mapper.TryMap(exception, out var details)) return details!;
        }

        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred."
        };
    }

    private static readonly List<ExceptionMapper> Mappers = [];

    private static void Map<TException>(Func<TException, ProblemDetails> mapping) where TException : Exception
    {
        var mapper = new ExceptionMapper(typeof(TException), exception => mapping((TException)exception));
        Mappers.Add(mapper);
    }

    private sealed class ExceptionMapper(Type type, Func<Exception, ProblemDetails> mapping)
    {
        public bool TryMap(Exception exception, out ProblemDetails? problem)
        {
            if (type.IsInstanceOfType(exception))
            {
                problem = mapping.Invoke(exception);
                return true;
            }

            problem = null;
            return false;
        }
    }
}
