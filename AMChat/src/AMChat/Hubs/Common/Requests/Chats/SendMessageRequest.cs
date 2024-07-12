namespace AMChat.Hubs.Common.Requests.Chats;

public record SendMessageRequest
{
    private readonly string _text = default!;

    public required Guid ChatId { get; init; }
    public required Guid AuthorId { get; init; }

    public required string Text
    {
        get => _text;
        init => _text = value.Trim();
    }
}
