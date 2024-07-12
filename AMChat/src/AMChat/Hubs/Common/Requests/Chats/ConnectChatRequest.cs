namespace AMChat.Hubs.Common.Requests.Chats;

public record ConnectChatRequest
{
    public required Guid ChatId { get; init; }
}
