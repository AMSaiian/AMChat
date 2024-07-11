using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryValidator : AbstractValidator<GetUsersPaginatedQuery>
{
    public GetUsersPaginatedQueryValidator(IValidator<PaginationContext> paginationContextValidator)
    {
        RuleFor(query => query.PaginationContext)
            .SetValidator(paginationContextValidator);

        RuleFor(query => query.SearchPattern)
            .NotNull();
    }
}
