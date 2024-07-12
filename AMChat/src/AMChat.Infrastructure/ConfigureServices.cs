using System.Globalization;
using AMChat.Application.Common.Interfaces;
using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence;
using AMChat.Infrastructure.Persistence.Interceptors;
using AMChat.Infrastructure.Persistence.Seeding.Fakers;
using AMChat.Infrastructure.Persistence.Seeding.Initializers;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace AMChat.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                       string connectionString,
                                                       int seedingValue = 42)
    {
        services
            .AddAppDbContext(connectionString)
            .AddAppDbContextInitializer(seedingValue);

        return services;
    }

    public static IServiceCollection AddAppDbContext(this IServiceCollection services,
                                                      string connectionString)
    {
        services
            .AddSingleton<SaveChangesInterceptor, SoftDeleteInterceptor>()
            .AddDbContext<AppDbContext>((provider, options) =>
            {
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention(CultureInfo.InvariantCulture)
                    .AddInterceptors(provider.GetRequiredService<SaveChangesInterceptor>());
            })
            .AddScoped<IAppDbContext, AppDbContext>();

        return services;
    }

    private static IServiceCollection AddAppDbContextInitializer(this IServiceCollection services,
                                                                 int seedingValue)
    {
        Randomizer.Seed = new Random(seedingValue);

        services
            .AddScoped<IAppDbContextInitializer, AppDbContextInitializer>()
            .AddScoped<Faker<Chat>, ChatFaker>()
            .AddScoped<Faker<User>, UserFaker>()
            .AddScoped<Faker<Message>, MessageFaker>()
            .AddScoped<Faker<Profile>, ProfileFaker>();

        return services;
    }
}
