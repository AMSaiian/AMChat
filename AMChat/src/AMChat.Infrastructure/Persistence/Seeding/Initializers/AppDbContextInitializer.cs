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
    private readonly ILogger<AppDbContextInitializer> _logger = logger;
    private readonly AppDbContext _context = context;

    private readonly Faker<Chat> _chatFaker = chatFaker;
    private readonly Faker<Message> _messageFaker = messageFaker;
    private readonly Faker<Profile> _profileFaker = profileFaker;
    private readonly Faker<User> _userFaker = userFaker;

    private readonly List<Chat> _chats = new();
    private readonly List<Message> _messages = new();
    private readonly List<Profile> _profiles = new();
    private readonly List<User> _users = new();

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

        _users.AddRange(generatedUsers);
        _profiles.AddRange(generatedProfiles);

        for (int i = 0; i < _userAmount; i++)
        {
            _users[i].Profile = _profiles[i];
        }

        await _context.AddRangeAsync(_users);
    }

    private async Task SeedChats()
    {
        _chats.AddRange(_chatFaker
                            .Generate(_chatsAmount));

        for (int i = 0; i < _chatsAmount; i++)
        {
            _chats[i].Owner = _users[i];
        }

        var joinedUsersChunks = _users
            .Skip(_chatsAmount)
            .Chunk(_joinedUsersPerChat)
            .ToList();

        for (int i = 0; i < joinedUsersChunks.Count; i++)
        {
            _chats[i].JoinedUsers.AddRange(joinedUsersChunks[i]);
        }

        await _context.Chats.AddRangeAsync(_chats);
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

        _messages.AddRange(_messageFaker
                               .Generate(totalMessagesAmountWithJoinChatMessages)
                               .OrderBy(message => message.PostedTime));

        for (int i = 0; i < chatCreationMessagesAmount; i++)
        {
            _messages[i].Text = string.Format(SystemMessageTemplates.CreateChatMessage,
                                              _chats[i].Owner.Name);

            _chats[i].Messages.Add(_messages[i]);
        }

        var joinToChatMessagesChunks = _messages
            .Skip(_chatsAmount)
            .Take(joinChatMessagesAmount)
            .Chunk(_joinedUsersPerChat)
            .ToList();

        for (int i = 0; i < _chatsAmount; i++)
        {
            for (int j = 0; j < _joinedUsersPerChat; j++)
            {
                joinToChatMessagesChunks[i][j].Text = string.Format(SystemMessageTemplates.JoinToChatMessage,
                                                                    _chats[i].JoinedUsers[j].Name);
                _chats[i].Messages.Add(joinToChatMessagesChunks[i][j]);
            }
        }

        var commonChatMessagesChunks = _messages
            .Skip(chatCreationMessagesAmount + joinChatMessagesAmount)
            .Chunk(commonMessagesInChatAmount)
            .ToList();

        for (int i = 0; i < _chatsAmount; i++)
        {
            int messageInChatIndex = 0;
            for (int j = 0; j < _messagesPerUser; j++)
            {
                _chats[i].Messages.Add(
                    AssignAuthorToMessage(commonChatMessagesChunks[i][messageInChatIndex++],
                                          _chats[i].Owner));

                for (int k = 0; k < _joinedUsersPerChat; k++)
                {
                    _chats[i].Messages.Add(
                        AssignAuthorToMessage(commonChatMessagesChunks[i][messageInChatIndex++],
                                              _chats[i].JoinedUsers[k]));
                }
            }
        }

        await _context.Messages.AddRangeAsync(_messages);
    }

    private Message AssignAuthorToMessage(Message message, User user)
    {
        message.Author = user;
        message.Kind = MessageKind.User;

        return message;
    }
}
