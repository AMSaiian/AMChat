using AMChat.Application.Common.Models.Pagination;
using FluentValidation;

namespace AMChat.Application.Common.Validators;

public class OrderContextValidator : AbstractValidator<OrderContext>
{
    public OrderContextValidator()
    {
        RuleFor(context => context.PropertyName)
            .NotEmpty();
    }
}
