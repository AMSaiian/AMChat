using AMChat.Application.Common.Models.Pagination;
using AMChat.Core.Entities;
using AMChat.Core.Interfaces;

namespace AMChat.Application.Common.Interfaces;

public interface IPaginationService
{
    public Task<Paginated<TEntity>> PaginateAsync<TEntity>(
        IQueryable<TEntity> query,
        PaginationContext context,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity, IOrdering;

    public IOrderedQueryable<TEntity> TryOrderDynamically<TEntity>(
        IQueryable<TEntity> query,
        OrderContext context)
        where TEntity : IOrdering;

    public IQueryable<TEntity> GetChunkQuery<TEntity>(
        IQueryable<TEntity> query,
        PageContext context)
        where TEntity : BaseEntity;

    public Task<PaginationInfo> GetPaginationInfoAsync<TEntity>(
        IQueryable<TEntity> initialQuery,
        PageContext context,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity;
}
