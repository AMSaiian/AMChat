using FluentValidation;

namespace AMChat.Application.Chats.Command.JoinChat;

public sealed class JoinChatCommandValidator : AbstractValidator<JoinChatCommand>
{
    public JoinChatCommandValidator()
    {
        RuleFor(command => command.ChatId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();
    }
}
