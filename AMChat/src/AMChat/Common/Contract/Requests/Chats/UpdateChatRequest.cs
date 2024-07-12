namespace AMChat.Common.Contract.Requests.Chats;

public record UpdateChatRequest
{
    private readonly string? _name;
    private readonly string? _description;

    public string? Name
    {
        get => _name;
        init => _name = value?.Trim();
    }

    public string? Description
    {
        get => _description;
        init => _description = value?.Trim();
    }

    public bool HasDescription { get; init; } = false;

    public Guid? OwnerId { get; init; }
}
