using AMChat.Application.Common.Interfaces;
using AutoMapper;

namespace AMChat.IntegrationTests.Common;

public abstract class BaseApiTests(IntegrationTestWebAppFactory factory)
    : IClassFixture<IntegrationTestWebAppFactory>
{
    protected IntegrationTestWebAppFactory Factory { get; } = factory;
    protected HttpClient HttpClient { get; } = factory.CreateClient();
    protected IMapper Mapper { get; } = factory.Scope.ServiceProvider.GetRequiredService<IMapper>();
    protected IAppDbContext DbContext { get; } =
        factory.Scope.ServiceProvider.GetRequiredService<IAppDbContext>();

    protected virtual async Task SetupDatabase()
    {
        await Factory.DbInitializer.ApplyDatabaseStructure();
        await Factory.DbInitializer.ClearStorageAsync();
        await Factory.DbInitializer.SeedAsync();
    }
}
