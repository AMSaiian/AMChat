namespace AMChat.Common.Contract.Requests.Users;

public record UpdateUserProfileRequest
{
    private readonly string? _fullname;
    private readonly string? _description;

    public string? Fullname
    {
        get => _fullname;
        init => _fullname = value?.Trim();
    }

    public string? Description
    {
        get => _description;
        init => _description = value?.Trim();
    }

    public bool HasDescription { get; init; } = false;

    public DateOnly? Birthdate { get; init; }
}
