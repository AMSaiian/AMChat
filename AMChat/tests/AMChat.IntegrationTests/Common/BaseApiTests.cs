using AutoMapper;

namespace AMChat.IntegrationTests.Common;

public class BaseApiTests(IntegrationTestWebAppFactory factory)
    : IClassFixture<IntegrationTestWebAppFactory>
{
    protected IntegrationTestWebAppFactory Factory { get; init; } = factory;
    protected HttpClient HttpClient { get; init; } = factory.CreateClient();
    protected IMapper Mapper { get; init; } = factory.Scope.ServiceProvider.GetRequiredService<IMapper>();

    protected virtual async Task SetupDatabase()
    {
        await Factory.DbInitializer.ApplyDatabaseStructure();
        await Factory.DbInitializer.ClearAsync();
        await Factory.DbInitializer.SeedAsync();
    }
}
