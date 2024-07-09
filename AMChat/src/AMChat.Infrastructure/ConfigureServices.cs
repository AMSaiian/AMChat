using System.Globalization;
using AMChat.Infrastructure.Persistence;
using AMChat.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace AMChat.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<SaveChangesInterceptor, SoftDeleteInterceptor>();

        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention(CultureInfo.InvariantCulture)
                .AddInterceptors(provider.GetRequiredService<SaveChangesInterceptor>());
        });

        return services;
    }
}
