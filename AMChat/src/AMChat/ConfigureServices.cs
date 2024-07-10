using System.Reflection;
using AMChat.Filters;
using AMChat.Middlewares;

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
                               configuration.AddMaps(Assembly.GetExecutingAssembly()));

        return services;
    }
}
