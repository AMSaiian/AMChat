using AMChat.Core.Entities;

namespace AMChat.Infrastructure.Persistence.Seeding.Initializers;

public interface IAppDbContextInitializer
{
    public List<Chat> Chats { get; init; }

    public List<Message> Messages { get; init; }

    public List<Profile> Profiles { get; init; }

    public List<User> Users { get; init; }

    public Task ApplyDatabaseStructure();

    public Task SeedAsync();

    public Task ClearStorageAsync();
}
