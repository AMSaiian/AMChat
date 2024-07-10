using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AMChat.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                                Exception exception,
                                                CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occured");

        ProblemDetails details = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = $"Unhandled exception occured {exception.GetType().Name}",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);

        return true;
    }
}
