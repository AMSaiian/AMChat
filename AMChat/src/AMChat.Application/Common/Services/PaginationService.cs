using AMChat.Application.Common.Exceptions;
using AMChat.Application.Common.Interfaces;
using AMChat.Application.Common.Models.Pagination;
using AMChat.Core.Interfaces;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Common.Services;

public class PaginationService : IPaginationService
{
    public async Task<Paginated<TEntity>> PaginateAsync<TEntity>(
        IQueryable<TEntity> query,
        PaginationContext context,
        CancellationToken cancellationToken)
        where TEntity : IOrdering
    {
        TEntity.OrderedBy.TryGetValue(context.OrderContext.PropertyName, out dynamic? orderExpression);

        if (orderExpression is null)
        {
            throw new ValidationException([ new ValidationFailure
            {
                PropertyName = nameof(context.OrderContext.PropertyName),
                ErrorMessage = $"Can't order entity by {context.OrderContext.PropertyName}"
            }]);
        }

        IQueryable<TEntity> sortedQuery = context.OrderContext.IsDescending
            ? Queryable.OrderByDescending(query, orderExpression)
            : Queryable.OrderBy(query, orderExpression);

        Paginated<TEntity> result = await PaginateAsync(query, sortedQuery, context, cancellationToken);

        return result;
    }

    private async Task<Paginated<T>> PaginateAsync<T>(IQueryable<T> query,
                                                      IQueryable<T> sortedQuery,
                                                      PaginationContext context,
                                                      CancellationToken cancellationToken)
    {
        int totalRecords = await query.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling((decimal)totalRecords / context.PageContext.PageSize);

        int skipAmount = (context.PageContext.PageNumber - 1) * context.PageContext.PageSize;

        sortedQuery = sortedQuery
            .Skip(skipAmount)
            .Take(context.PageContext.PageSize);

        List<T> entities = await sortedQuery.ToListAsync(cancellationToken);

        Paginated<T> paginated = new()
        {
            PaginationInfo = new()
            {
                PageNumber = context.PageContext.PageNumber,
                PageSize = context.PageContext.PageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords,
            },
            Records = entities
        };

        return paginated;
    }
}
