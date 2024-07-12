namespace AMChat.Hubs.Common.Models.HubUser;

public record HubUser
{
    public required string Id { get; init; }
    public List<string> ConnectionIds { get; init; } = [];
};
