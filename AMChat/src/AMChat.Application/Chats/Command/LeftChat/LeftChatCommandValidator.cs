using FluentValidation;

namespace AMChat.Application.Chats.Command.LeftChat;

public class LeftChatCommandValidator : AbstractValidator<LeftChatCommand>
{
    public LeftChatCommandValidator()
    {
        RuleFor(command => command.ChatId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();
    }
}
