using System.Reflection;
using AMChat.Application.Common.Interfaces;
using AMChat.Common.Services;
using AMChat.Filters;
using AMChat.Hubs.Common.Cache;
using AMChat.Hubs.Common.Interfaces;
using AMChat.Middlewares;
using Microsoft.AspNetCore.SignalR;

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

        services.AddSignalR(options =>
        {
            options.AddFilter<SignalrExceptionFilter>();
        });

        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerUserIdFilter>();
            })
            .AddAutoMapper(configuration =>
                               configuration.AddMaps(Assembly.GetExecutingAssembly()))
            .AddCustomServices();

        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services
            .AddSingleton<ICurrentUserService, CurrentUserService>()
            .AddSingleton<IChatCache, ChatCache>()
            .AddScoped<IChatService, ChatService>();

        return services;
    }
}
