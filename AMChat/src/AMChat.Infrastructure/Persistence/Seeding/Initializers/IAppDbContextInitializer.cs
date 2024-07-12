namespace AMChat.Infrastructure.Persistence.Seeding.Initializers;

public interface IAppDbContextInitializer
{
    public Task ApplyDatabaseStructure();

    public Task SeedAsync();
}
