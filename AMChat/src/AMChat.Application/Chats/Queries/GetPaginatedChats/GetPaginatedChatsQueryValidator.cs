using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Chats.Queries.GetPaginatedChats;

public class GetPaginatedChatsQueryValidator : AbstractValidator<GetPaginatedChatsQuery>
{
    public GetPaginatedChatsQueryValidator(IValidator<PaginationContext> paginationContextValidator)
    {
        RuleFor(query => query.PaginationContext)
            .SetValidator(paginationContextValidator);

        RuleFor(query => query.SearchPattern)
            .NotNull();
    }
}
