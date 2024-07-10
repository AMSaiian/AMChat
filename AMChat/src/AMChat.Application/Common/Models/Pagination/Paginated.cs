namespace AMChat.Application.Common.Models.Pagination;

public record Paginated<T>
{
    public required PaginationInfo PaginationInfo { get; init; }
    public required List<T> Records { get; init; }
}
