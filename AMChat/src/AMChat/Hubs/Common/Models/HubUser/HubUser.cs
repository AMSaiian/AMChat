namespace AMChat.Hubs.Common.Models.HubUser;

public record HubUser
{
    public required string Id { get; init; }
    public HashSet<string> ConnectionIds { get; init; } = [];
};
