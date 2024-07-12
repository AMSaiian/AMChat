namespace AMChat.Application.Common.Models.User;

public record UserDetailedDto : UserDto
{
    public required DateOnly ProfileBirthdate { get; init; }
    public string? ProfileDescription { get; init; }
    public required List<Guid> OwnedChatsId { get; init; }
    public required List<Guid> JoinedChatsId { get; init; }
}
