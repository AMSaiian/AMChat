using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Common.Validators;

public class PageContextValidator : AbstractValidator<PageContext>
{
    public PageContextValidator()
    {
        RuleFor(context => context.PageNumber)
            .GreaterThan(0);

        RuleFor(context => context.PageSize)
            .GreaterThan(0);
    }
}
