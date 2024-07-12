using AMChat.Application.Chats.Command.JoinChat;
using AMChat.Core.Entities;
using AMChat.Core.Enums;
using AMChat.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AMChat.UnitTests.Tests;

public class ChatTests : UnitTestBase
{
    [Fact]
    public async Task JoinChatCommandCreateSystemMessageAndAddUserToChat()
    {
        // Arrange
        await SetupDbContext();

        Chat affectingChat = AppDbContextInitializer.Chats[0];
        User affectingUser = AppDbContextInitializer.Users.First(user => !user.JoinedChats.Contains(affectingChat)
                                                                      && !user.OwnedChats.Contains(affectingChat));

        CurrentUserServiceMoq.Reset();
        CurrentUserServiceMoq
            .Setup(service => service.GetUserIdOrThrow())
            .Returns(affectingUser.Id);
        CurrentUserServiceMoq
            .Setup(service => service.UserId)
            .Returns(affectingUser.Id);

        JoinChatCommand command = new()
        {
            ChatId = affectingChat.Id,
            UserId = affectingUser.Id
        };
        JoinChatHandler handler = new(DbContext,
                                      CurrentUserServiceMoq.Object,
                                      Mapper,
                                      ChatServiceMoq.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Chat affectedChat = await DbContext.Chats
            .Include(chat => chat.JoinedUsers)
            .Include(chat => chat.Messages)
            .FirstAsync(chat => chat.Id == affectingChat.Id);

        affectedChat.JoinedUsers
            .Should()
            .Contain(user => user.Id == affectingUser.Id);

        List<Message> affectedChatMessages = affectedChat.Messages
            .OrderBy(message => message.PostedTime)
            .ToList();

        affectedChatMessages[^1].Should().BeEquivalentTo<Message>(new()
        {
            ChatId = affectingChat.Id,
            Author = null,
            AuthorId = null,
            Kind = MessageKind.System,
            Text = $"User {affectingUser.Name} joined to the chat"
        }, options => options
            .Excluding(message => message.PostedTime)
            .Excluding(message => message.Chat)
            .Excluding(message => message.Id));
    }
}
