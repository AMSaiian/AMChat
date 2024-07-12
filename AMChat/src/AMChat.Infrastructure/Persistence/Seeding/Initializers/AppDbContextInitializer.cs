using AMChat.Application.Common.Templates;
using AMChat.Core.Entities;
using AMChat.Core.Enums;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AMChat.Infrastructure.Persistence.Seeding.Initializers;

public class AppDbContextInitializer(ILogger<AppDbContextInitializer> logger,
                                     AppDbContext context,
                                     Faker<Chat> chatFaker,
                                     Faker<Message> messageFaker,
                                     Faker<Profile> profileFaker,
                                     Faker<User> userFaker)
    : IAppDbContextInitializer
{
    public List<Chat> Chats { get; init; } = new();

    public List<Message> Messages { get; init; } = new();

    public List<Profile> Profiles { get; init; } = new();

    public List<User> Users { get; init; } = new();

    private readonly ILogger<AppDbContextInitializer> _logger = logger;
    private readonly AppDbContext _context = context;

    private readonly Faker<Chat> _chatFaker = chatFaker;
    private readonly Faker<Message> _messageFaker = messageFaker;
    private readonly Faker<Profile> _profileFaker = profileFaker;
    private readonly Faker<User> _userFaker = userFaker;

    private readonly int _userAmount = 20;
    private readonly int _chatsAmount = 5;
    private readonly int _joinedUsersPerChat = 3;
    private readonly int _messagesPerUser = 5;

    public async Task ApplyDatabaseStructure()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        if (!await CanBeSeeded())
        {
            return;
        }

        try
        {
            Users.Clear();
            Profiles.Clear();
            Messages.Clear();
            Chats.Clear();

            await SeedUsersAndProfiles();
            await SeedChats();
            await SeedMessages();

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task ClearStorageAsync()
    {
        await _context.Messages.ExecuteDeleteAsync();
        await _context.Chats.ExecuteDeleteAsync();
        await _context.Profiles.ExecuteDeleteAsync();
        await _context.Users.ExecuteDeleteAsync();
    }

    private async Task<bool> CanBeSeeded()
    {
        return !(await _context.Users.AnyAsync()
              || await _context.Profiles.AnyAsync()
              || await _context.Messages.AnyAsync()
              || await _context.Chats.AnyAsync());
    }

    private async Task SeedUsersAndProfiles()
    {
        var generatedUsers = _userFaker.Generate(_userAmount);
        var generatedProfiles = _profileFaker.Generate(_userAmount);

        Users.AddRange(generatedUsers);
        Profiles.AddRange(generatedProfiles);

        for (int i = 0; i < _userAmount; i++)
        {
            Users[i].Profile = Profiles[i];
        }

        await _context.AddRangeAsync(Users);
    }

    private async Task SeedChats()
    {
        Chats.AddRange(_chatFaker
                            .Generate(_chatsAmount));

        var chatOwners = Users
            .Take(_chatsAmount)
            .ToList();

        for (int i = 0; i < _chatsAmount; i++)
        {
            Chats[i].Owner = chatOwners[i];
        }

        var joinedUsersChunks = Users
            .Skip(_chatsAmount)
            .Chunk(_joinedUsersPerChat)
            .ToList();

        for (int i = 0; i < joinedUsersChunks.Count; i++)
        {
            Chats[i].JoinedUsers.AddRange(joinedUsersChunks[i]);
            Chats[i].JoinedUsers.AddRange(chatOwners
                                               .Where(user => user != Chats[i].Owner));
        }

        await _context.Chats.AddRangeAsync(Chats);
    }

    private async Task SeedMessages()
    {
        int chatCreationMessagesAmount = _chatsAmount;
        int joinChatMessagesAmount = _joinedUsersPerChat * _chatsAmount;
        int commonMessagesInChatAmount = (_joinedUsersPerChat + 1) * _messagesPerUser;
        int commonMessagesAmount = commonMessagesInChatAmount * _chatsAmount;

        int totalMessagesAmountWithJoinChatMessages = chatCreationMessagesAmount
                                                    + joinChatMessagesAmount
                                                    + commonMessagesAmount;

        Messages.AddRange(_messageFaker
                               .Generate(totalMessagesAmountWithJoinChatMessages)
                               .OrderBy(message => message.PostedTime));

        for (int i = 0; i < chatCreationMessagesAmount; i++)
        {
            Messages[i].Text = string.Format(SystemMessageTemplates.CreateChatMessage,
                                              Chats[i].Owner.Name);

            Chats[i].Messages.Add(Messages[i]);
        }

        var joinToChatMessagesChunks = Messages
            .Skip(_chatsAmount)
            .Take(joinChatMessagesAmount)
            .Chunk(_joinedUsersPerChat)
            .ToList();

        for (int i = 0; i < _chatsAmount; i++)
        {
            for (int j = 0; j < _joinedUsersPerChat; j++)
            {
                joinToChatMessagesChunks[i][j].Text = string.Format(SystemMessageTemplates.JoinToChatMessage,
                                                                    Chats[i].JoinedUsers[j].Name);
                Chats[i].Messages.Add(joinToChatMessagesChunks[i][j]);
            }
        }

        var commonChatMessagesChunks = Messages
            .Skip(chatCreationMessagesAmount + joinChatMessagesAmount)
            .Chunk(commonMessagesInChatAmount)
            .ToList();

        for (int i = 0; i < _chatsAmount; i++)
        {
            int messageInChatIndex = 0;
            for (int j = 0; j < _messagesPerUser; j++)
            {
                Chats[i].Messages.Add(
                    AssignAuthorToMessage(commonChatMessagesChunks[i][messageInChatIndex++],
                                          Chats[i].Owner));

                for (int k = 0; k < _joinedUsersPerChat; k++)
                {
                    Chats[i].Messages.Add(
                        AssignAuthorToMessage(commonChatMessagesChunks[i][messageInChatIndex++],
                                              Chats[i].JoinedUsers[k]));
                }
            }
        }

        await _context.Messages.AddRangeAsync(Messages);
    }

    private Message AssignAuthorToMessage(Message message, User user)
    {
        message.Author = user;
        message.Kind = MessageKind.User;

        return message;
    }
}
