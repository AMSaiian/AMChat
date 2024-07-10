using AMChat.Application.Common.Models.Pagination;
using AMChat.Core.Interfaces;

namespace AMChat.Application.Common.Interfaces;

public interface IPaginationService
{
    public Task<Paginated<TEntity>> PaginateAsync<TEntity>(
        IQueryable<TEntity> query,
        PaginationContext context,
        CancellationToken cancellationToken)
        where TEntity : IOrdering;
}
