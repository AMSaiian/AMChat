using AMChat.Infrastructure;
using AMChat.Infrastructure.Persistence;
using AMChat.Infrastructure.Persistence.Seeding.Initializers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace AMChat.IntegrationTests.Common;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>,
                                            IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage(DbContainerConstants.ImageName)
        .WithDatabase(DbContainerConstants.DatabaseName)
        .WithUsername(DbContainerConstants.Username)
        .WithPassword(DbContainerConstants.Password)
        .Build();

    public IServiceScope Scope { get; set; } = default!;
    public IAppDbContextInitializer DbInitializer { get; set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddAppDbContext(_dbContainer.GetConnectionString());
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Scope = Services
            .CreateScope();
        DbInitializer = Scope.ServiceProvider
            .GetRequiredService<IAppDbContextInitializer>();
    }


    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        Scope.Dispose();
    }
}
