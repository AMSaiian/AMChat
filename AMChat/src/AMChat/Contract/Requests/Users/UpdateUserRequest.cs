namespace AMChat.Contract.Requests.Users;

public record UpdateUserRequest
{
    private readonly string _name = default!;

    public required string Name
    {
        get => _name;
        init => _name = value.Trim();
    }
}
