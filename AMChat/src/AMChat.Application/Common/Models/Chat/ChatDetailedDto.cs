namespace AMChat.Application.Common.Models.Chat;

public record ChatDetailedDto : ChatDto
{
    public required Guid OwnerId { get; init; }
    public required List<Guid> JoinedUsersId { get; init; }
    public string? Description { get; init; }
}
