namespace AMChat.Common.Contract.Queries;

public record PaginationQuery
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
