namespace AMChat.Application.Common.Models.Chat;

public record ChatDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string OwnerName { get; init; }
    public string? Description { get; init; }
}
