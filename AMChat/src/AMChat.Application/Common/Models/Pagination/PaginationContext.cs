namespace AMChat.Application.Common.Models.Pagination;

public record PaginationContext
{
    public required PageContext PageContext { get; init; }
    public required OrderContext OrderContext { get; init; }
}