namespace AMChat.Application.Common.Models.User;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ProfileFullname { get; init; }
    public required DateOnly ProfileBirthdate { get; init; }
    public string? ProfileDescription { get; init; }
    public required List<Guid> OwnedChatsId { get; init; }
    public required List<Guid> JoinedChatsId { get; init; }
}
