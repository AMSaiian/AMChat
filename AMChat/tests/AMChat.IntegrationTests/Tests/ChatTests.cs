using System.Net;
using AMChat.Application.Common.Models.Chat;
using AMChat.Core.Entities;
using AMChat.IntegrationTests.Common;
using FluentAssertions;

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

        // Act
        var requestUri = $"{ApiEndpointConstants.ApiBase}/{ApiEndpointConstants.ChatsBase}/{needChat.Id}";
        HttpResponseMessage response = await HttpClient.GetAsync(requestUri);

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        ChatDto? gotChat = await response.Content.ReadFromJsonAsync<ChatDto>();
        needChatDto.Should().Be(gotChat);
    }
}
