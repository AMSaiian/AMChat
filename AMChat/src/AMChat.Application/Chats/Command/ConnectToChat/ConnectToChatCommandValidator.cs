using FluentValidation;

namespace AMChat.Application.Chats.Command.ConnectToChat;

public sealed class ConnectToChatCommandValidator : AbstractValidator<ConnectToChatCommand>
{
    public ConnectToChatCommandValidator()
    {
        RuleFor(command => command.ChatId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.ConnectionId)
            .NotEmpty();
    }
}
