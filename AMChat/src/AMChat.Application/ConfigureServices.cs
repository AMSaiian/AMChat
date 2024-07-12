using System.Reflection;
using AMChat.Application.Common.Behaviours;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMChat.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddHandlersAndBehaviour()
            .AddFluentValidators()
            .AddMapping()
            .AddCustomServices();

        return services;
    }

    private static IServiceCollection AddHandlersAndBehaviour(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            options.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            options.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        return services;
    }

    private static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(configuration =>
                                   configuration.AddMaps(Assembly.GetExecutingAssembly()));

        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaginationService, PaginationService>();

        return services;
    }
}
