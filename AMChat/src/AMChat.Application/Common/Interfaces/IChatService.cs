using AMChat.Application.Common.Models.Message;

namespace AMChat.Application.Common.Interfaces;

public interface IChatService
{
    public Task SendMessagesToConnectionAsync(string connectionId, List<MessageDto> messages);

    public Task SendMessagesAsync(string chatId, List<MessageDto> messages);

    public Task ConnectToChatAsync(string chatId, string userId, string connectionId);

    public Task DisconnectAllUserConnectionsFromChat(string chatId, string userId);

    public Task DisconnectFromChatAsync(string chatId, string userId, string connectionId);

    public Task DisconnectFromAllChatsAsync(string userId);

    public Task DisconnectAllUsersFromChat(string chatId);
}
