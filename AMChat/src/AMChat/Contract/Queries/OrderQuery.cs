namespace AMChat.Contract.Queries;

public record OrderQuery
{
    public string? PropertyName { get; init; }
    public bool IsDescending { get; init; } = false;
}
