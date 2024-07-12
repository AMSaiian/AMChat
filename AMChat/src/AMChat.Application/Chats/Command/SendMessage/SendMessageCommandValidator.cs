using AMChat.Core;
using FluentValidation;

namespace AMChat.Application.Chats.Command.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(command => command.AuthorId)
            .NotEmpty();

        RuleFor(command => command.ChatId)
            .NotEmpty();

        RuleFor(command => command.Text)
            .NotEmpty()
            .MaximumLength(DomainConstraints.MessageTextLength);
    }
}
