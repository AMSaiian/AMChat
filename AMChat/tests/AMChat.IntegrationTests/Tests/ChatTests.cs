using System.Net;
using AMChat.Application.Common.Models.Chat;
using AMChat.Common.Constants;
using AMChat.Common.Contract.Requests.Chats;
using AMChat.Core.Entities;
using AMChat.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AMChat.IntegrationTests.Tests;

public class ChatTests(IntegrationTestWebAppFactory factory)
    : BaseApiTests(factory)
{
    [Fact]
    public async Task GetChatByValidIdMustReturnCorrectChat()
    {
        // Arrange
        await SetupDatabase();
        Chat needChat = Factory.DbInitializer.Chats[0];
        ChatDto needChatDto = Mapper.Map<ChatDto>(needChat);
        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{needChat.Id}";

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(requestUri);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        ChatDto? gotChat = await response.Content.ReadFromJsonAsync<ChatDto>();
        needChatDto.Should().Be(gotChat);
    }

    [Fact]
    public async Task GetChatByNotWellFormedIdMustReturnBadRequest()
    {
        // Arrange
        await SetupDatabase();
        string invalidGuid = "aafffasfasd";
        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{invalidGuid}";

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(requestUri);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetChatByInvalidIdMustReturnNotFound()
    {
        // Arrange
        await SetupDatabase();
        Guid invalidGuid = Guid.NewGuid();
        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{invalidGuid}";

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(requestUri);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateChatWithNoChangesObjectMustReturnBadRequest()
    {
        // Arrange
        await SetupDatabase();
        Chat updatingChat = Factory.DbInitializer.Chats[0];
        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{updatingChat.Id}";

        UpdateChatRequest request = new()
        {
            Name = null,
            Description = null,
            HasDescription = false,
            OwnerId = null
        };

        HttpClient.DefaultRequestHeaders.Add(CustomHeaders.UserIdHeader, updatingChat.Owner.Id.ToString());

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync(requestUri, request);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateChatWithOwnerChangeMustAddPreviousOwnerToJoinedAndJoinedUserToOwner()
    {
        // Arrange
        await SetupDatabase();

        Chat updatingChat = Factory.DbInitializer.Chats
            .First(chat => chat.JoinedUsers.Count >= 1);
        User previousOwner = updatingChat.Owner;
        User newOwner = updatingChat.JoinedUsers[0];

        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{updatingChat.Id}";

        UpdateChatRequest request = new()
        {
            Name = "NewTestName",
            Description = "NewTestDescription",
            HasDescription = true,
            OwnerId = newOwner.Id
        };

        HttpClient.DefaultRequestHeaders.Add(CustomHeaders.UserIdHeader, updatingChat.Owner.Id.ToString());

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync(requestUri, request);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        Chat updatedChat = await DbContext.Chats
            .Include(chat => chat.Owner)
            .Include(chat => chat.JoinedUsers)
            .AsNoTracking()
            .FirstAsync(chat => chat.Id == updatingChat.Id);

        updatedChat
            .Should()
            .BeEquivalentTo<Chat>(new()
            {
                Id = updatingChat.Id,
                Description = request.Description,
                Name = request.Name,
                OwnerId = request.OwnerId.Value
            }, options => options
                .Excluding(chat => chat.JoinedUsers)
                .Excluding(chat => chat.Messages)
                .Excluding(chat => chat.Owner));

        updatedChat.JoinedUsers
            .Should()
            .Contain(user => user.Id == previousOwner.Id);

        updatedChat.Owner.Id
            .Should()
            .Be(newOwner.Id);

        HttpClient.DefaultRequestHeaders.Remove(CustomHeaders.UserIdHeader);
    }

    [Fact]
    public async Task DeleteChatWithNotOwnerCredentialsMustReturnForbidden()
    {
        // Arrange
        await SetupDatabase();

        Chat deletingChat = Factory.DbInitializer.Chats[0];

        Guid randomCredentials = Guid.NewGuid();

        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{deletingChat.Id}";

        HttpClient.DefaultRequestHeaders.Add(CustomHeaders.UserIdHeader, randomCredentials.ToString());

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync(requestUri);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);

        Chat? notDeletedChat = await DbContext.Chats
            .Include(chat => chat.Owner)
            .Include(chat => chat.JoinedUsers)
            .AsNoTracking()
            .FirstOrDefaultAsync(chat => chat.Id == deletingChat.Id);

        notDeletedChat
            .Should()
            .BeEquivalentTo<Chat>(new()
            {
                Id = deletingChat.Id,
                Description = deletingChat.Description,
                Name = deletingChat.Name,
                OwnerId = deletingChat.OwnerId
            }, options => options
                .Excluding(chat => chat.JoinedUsers)
                .Excluding(chat => chat.Messages)
                .Excluding(chat => chat.Owner));

        HttpClient.DefaultRequestHeaders.Remove(CustomHeaders.UserIdHeader);
    }

    [Fact]
    public async Task DeleteChatWithOwnerCredentialsMustRemoveChatFromStorage()
    {
        // Arrange
        await SetupDatabase();

        Chat deletingChat = Factory.DbInitializer.Chats[0];

        Guid ownerCredentials = deletingChat.OwnerId;

        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{deletingChat.Id}";

        HttpClient.DefaultRequestHeaders.Add(CustomHeaders.UserIdHeader, ownerCredentials.ToString());

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync(requestUri);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        bool isChatDeleted = !await DbContext.Chats
            .AsNoTracking()
            .AnyAsync(chat => chat.Id == deletingChat.Id);

        isChatDeleted.Should().Be(true);

        HttpClient.DefaultRequestHeaders.Remove(CustomHeaders.UserIdHeader);
    }
}
