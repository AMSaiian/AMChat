using AMChat.Application.Users.Queries.GetUserById;
using FluentValidation;

namespace AMChat.Application.Chats.Queries.GetChatById;

public sealed class GetChatQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetChatQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty();
    }
}
