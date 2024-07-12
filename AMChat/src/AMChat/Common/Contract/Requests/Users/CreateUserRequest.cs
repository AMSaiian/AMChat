namespace AMChat.Common.Contract.Requests.Users;

public record CreateUserRequest
{
    private readonly string _name = default!;
    private readonly string _profileFullname = default!;
    private readonly string? _profileDescription;

    public required string Name
    {
        get => _name;
        init => _name = value.Trim();
    }

    public required string ProfileFullname
    {
        get => _profileFullname;
        init => _profileFullname = value.Trim();
    }

    public string? ProfileDescription
    {
        get => _profileDescription;
        init => _profileDescription = value?.Trim();
    }

    public required DateOnly ProfileBirthdate { get; init; }
}
