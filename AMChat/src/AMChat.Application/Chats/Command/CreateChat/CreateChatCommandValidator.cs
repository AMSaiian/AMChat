using AMChat.Core;
using FluentValidation;

namespace AMChat.Application.Chats.Command.CreateChat;

public sealed class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(command => command.OwnerId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(DomainConstraints.ChatNameLength);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(DomainConstraints.ChatDescriptionLength)
            .When(command => command.Description is not null);
    }
}
