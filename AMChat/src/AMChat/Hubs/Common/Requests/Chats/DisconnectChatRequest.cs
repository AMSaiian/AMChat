namespace AMChat.Hubs.Common.Requests.Chats;

public record DisconnectChatRequest
{
    public required Guid ChatId { get; init; }
}
