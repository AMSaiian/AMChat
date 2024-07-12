using FluentValidation;

namespace AMChat.Application.Chats.Command.DisconnectFromChat;

public sealed class DisconnectFromChatCommandValidator : AbstractValidator<DisconnectFromChatCommand>
{
    public DisconnectFromChatCommandValidator()
    {
        RuleFor(command => command.ChatId)
            .NotEmpty();

        RuleFor(command => command.ConnectionId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();
    }
}
