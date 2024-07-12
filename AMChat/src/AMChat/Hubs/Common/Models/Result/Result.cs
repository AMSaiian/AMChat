namespace AMChat.Hubs.Common.Models.Result;

public record Result
{
    public ResultType Type { get; init; } = ResultType.Success;
    public object? Value { get; init; }
    public List<string> Errors { get; init; } = [];
}
