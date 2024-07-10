using System.Reflection;
using AMChat.Application.Common.Interfaces;
using AMChat.Filters;
using AMChat.Middlewares;
using AMChat.Services;

namespace AMChat;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddProblemDetails();

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddControllers(opts => opts.Filters.Add<ApiExceptionFilterAttribute>());

        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddAutoMapper(configuration =>
                               configuration.AddMaps(Assembly.GetExecutingAssembly()))
            .AddCustomServices();

        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services
            .AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
