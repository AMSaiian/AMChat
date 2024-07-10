using AMChat.Application.Common.Templates;
using AMChat.Core;
using FluentValidation;

namespace AMChat.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(DomainConstraints.UsernameLength);

        RuleFor(command => command.ProfileDescription)
            .NotEmpty()
            .When(command => command.ProfileDescription is not null);

        RuleFor(command => command.ProfileFullname)
            .NotEmpty()
            .MaximumLength(DomainConstraints.FullnameLength);

        RuleFor(command => command.ProfileBirthdate)
            .LessThanOrEqualTo(DateOnly
                                      .FromDateTime(DateTime.UtcNow
                                                        .AddYears(-DomainConstraints.MinimalUserAge)))
            .WithMessage(ErrorTemplates.MinimumUserAgeViolated);
    }
}
