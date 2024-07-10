using AMChat.Application.Common.Templates;
using AMChat.Core;
using FluentValidation;

namespace AMChat.Application.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command)
            .Must(command => command.Fullname is not null
                          || command.Birthdate is not null
                          || command.HasDescription)
            .WithMessage(ErrorTemplates.EmptyUpdateObjectError);

        RuleFor(command => command.Fullname)
            .NotEmpty()
            .MaximumLength(DomainConstraints.FullnameLength)
            .When(command => command.Fullname is not null);

        RuleFor(command => command.Birthdate)
            .LessThanOrEqualTo(DateOnly
                                   .FromDateTime(DateTime.UtcNow
                                                     .AddYears(-DomainConstraints.MinimalUserAge)))
            .WithMessage(ErrorTemplates.MinimumUserAgeViolated);

        When(command => command.HasDescription
                     && command.Description is not null,
             () =>
             {
                 RuleFor(command => command.Description)
                     .NotEmpty()
                     .MaximumLength(DomainConstraints.ProfileDescription);
             });
    }
}
