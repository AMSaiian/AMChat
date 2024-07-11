namespace AMChat.Contract.Requests.Chats;

public record CreateChatRequest
{
    private readonly string _name = default!;
    private readonly string? _description;

    public required string Name
    {
        get => _name;
        init => _name = value.Trim();
    }

    public string? Description
    {
        get => _description;
        init => _description = value?.Trim();
    }

    public required Guid OwnerId { get; init; }
}
