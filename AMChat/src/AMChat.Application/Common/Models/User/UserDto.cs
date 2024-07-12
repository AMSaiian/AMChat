namespace AMChat.Application.Common.Models.User;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ProfileFullname { get; init; }
}
