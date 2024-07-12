using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator(IValidator<OrderContext> orderContextValidator)
    {
        RuleFor(query => query.OrderContext)
            .SetValidator(orderContextValidator)
            .When(query => query.OrderContext is not null);

        RuleFor(query => query.SearchPattern)
            .NotNull();
    }
}
