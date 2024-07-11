using AMChat.Application.Common.Templates;
using AMChat.Core;
using FluentValidation;

namespace AMChat.Application.Chats.Command.UpdateChat;

public sealed class UpdateChatCommandValidator : AbstractValidator<UpdateChatCommand>
{
    public UpdateChatCommandValidator()
    {
        RuleFor(command => command)
            .Must(command => command.Name is not null
                          || command.OwnerId is not null
                          || command.HasDescription)
            .WithMessage(ErrorTemplates.EmptyUpdateObjectError);

        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.OwnerId)
            .NotEmpty()
            .When(command => command.OwnerId is not null);

        When(command => command.HasDescription
                     && command.Description is not null,
             () =>
             {
                 RuleFor(command => command.Description)
                     .NotEmpty()
                     .MaximumLength(DomainConstraints.ChatDescriptionLength);
             });
    }
}
