using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Common.Validators;

public class PaginationContextValidator : AbstractValidator<PaginationContext>
{
    public PaginationContextValidator(IValidator<OrderContext> orderContextValidator,
                                      IValidator<PageContext> pageContextValidator)
    {
        RuleFor(context => context.PageContext)
            .NotNull()
            .SetValidator(pageContextValidator);

        RuleFor(context => context.OrderContext)
            .NotNull()
            .SetValidator(orderContextValidator);
    }
}
