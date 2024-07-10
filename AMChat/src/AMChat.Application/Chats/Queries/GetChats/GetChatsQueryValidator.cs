using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Chats.Queries.GetChats;

public sealed class GetChatsQueryValidator : AbstractValidator<GetChatsQuery>
{
    public GetChatsQueryValidator(IValidator<OrderContext> orderContextValidator)
    {
        RuleFor(query => query.OrderContext)
            .SetValidator(orderContextValidator)
            .When(query => query.OrderContext is not null);

        RuleFor(query => query.SearchPattern)
            .NotNull();
    }
}
