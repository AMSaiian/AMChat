using MediatR;
using Microsoft.Extensions.Logging;

namespace AMChat.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(TRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        _logger.LogInformation("Processing Request {@Request} at {@DateTime}",
                               request,
                               DateTime.UtcNow);

        try
        {
            TResponse response = await next();
            _logger.LogInformation("Processed Request successfully {@RequestName} "
                                 + "with result: {@Response} at {@DateTime}",
                                   requestName,
                                   response,
                                   DateTime.UtcNow);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request {@Request} failed with error at {@DateTime}",
                             request,
                             DateTime.UtcNow);
            throw;
        }
    }
}
