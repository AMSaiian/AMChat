using FluentValidation;

namespace AMChat.Application.Chats.Command.DeleteChat;

public sealed class DeleteChatCommandValidator : AbstractValidator<DeleteChatCommand>
{
    public DeleteChatCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
