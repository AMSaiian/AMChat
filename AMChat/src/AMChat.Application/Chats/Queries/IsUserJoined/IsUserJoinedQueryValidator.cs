using FluentValidation;

namespace AMChat.Application.Chats.Queries.IsUserJoined;

public sealed class IsUserJoinedQueryValidator : AbstractValidator<IsUserJoinedChatQuery>
{
    public IsUserJoinedQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.ChatId)
            .NotEmpty();
    }
}
