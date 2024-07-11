namespace AMChat.Hubs.Common.Models.Result;

public record Result
{
    public ResultType Type { get; init; } = ResultType.Success;
    public List<string> Errors { get; init; } = [];
}
