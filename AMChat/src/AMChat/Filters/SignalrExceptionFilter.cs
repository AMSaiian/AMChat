using AMChat.Application.Common.Exceptions;
using AMChat.Hubs.Common.Models.Result;
using Microsoft.AspNetCore.SignalR;
using UnauthorizedAccessException = AMChat.Application.Common.Exceptions.UnauthorizedAccessException;

namespace AMChat.Filters;

public class SignalrExceptionFilter : IHubFilter
{
    private readonly IReadOnlyDictionary<Type, Func<Exception, Result>> _exceptionHandlers;
    private readonly ILogger<SignalrExceptionFilter> _logger;

    public SignalrExceptionFilter(ILogger<SignalrExceptionFilter> logger)
    {
        _logger = logger;

        _exceptionHandlers = new Dictionary<Type, Func<Exception, Result>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(ConflictException), HandleConflictException },
        };
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext,
                                                      Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            return await HandleException(ex);
        }
    }

    private Task<Result> HandleException(Exception ex)
    {
        Type type = ex.GetType();

        if (_exceptionHandlers.TryGetValue(type, out Func<Exception, Result>? handler))
        {
            return Task.FromResult(handler(ex));
        }
        else
        {
            return Task.FromResult(HandleUnhandledException(ex));
        }
    }

    private Result HandleConflictException(Exception ex)
    {
        var exception = (ConflictException)ex;

        return new()
        {
            Type = ResultType.ConflictError,
            Errors = [exception.Message]
        };
    }

    private Result HandleForbiddenAccessException(Exception ex)
    {
        var exception = (ForbiddenAccessException)ex;

        return new()
        {
            Type = ResultType.ForbiddenError,
            Errors = [exception.Message]
        };
    }

    private Result HandleUnauthorizedAccessException(Exception ex)
    {
        var exception = (UnauthorizedAccessException)ex;

        return new()
        {
            Type = ResultType.UnauthorisedError,
            Errors = [exception.Message]
        };
    }

    private Result HandleNotFoundException(Exception ex)
    {
        var exception = (NotFoundException)ex;

        return new()
        {
            Type = ResultType.NotFoundError,
            Errors = [exception.Message]
        };
    }

    private Result HandleValidationException(Exception ex)
    {
        var exception = (ValidationException)ex;

        return new()
        {
            Type = ResultType.ValidationError,
            Errors = exception.Errors
                .SelectMany(pair =>
                                pair.Value
                                    .Select(error => $"{pair.Key} - {error}"))
                .ToList()
        };
    }

    private Result HandleUnhandledException(Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occured during processing SignalR request");

        return new()
        {
            Type = ResultType.InternalError,
            Errors = [ex.Message]
        };
    }
}
